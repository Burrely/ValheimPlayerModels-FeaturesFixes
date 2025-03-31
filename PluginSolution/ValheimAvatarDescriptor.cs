﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ValheimPlayerModels
{
    public enum ControlType
    {
        Button,
        Toggle,
        Slider
    }

    public enum ValheimAvatarParameterType
    {
        Bool,
        Int,
        Float
    }

    [Serializable]
    public struct ValheimAvatarParameter
    {
        public string name;
        public ValheimAvatarParameterType type;
        public float defaultValue;
    }
    
    public class ValheimAvatarDescriptor : MonoBehaviour, ISerializationCallbackReceiver
    {
        public string avatarName = "player";

        public Transform leftHand;
        public Transform rightHand;
        public Transform helmet;
        public Transform backShield;
        public Transform backMelee;
        public Transform backTwohandedMelee;
        public Transform backBow;
        public Transform backTool;
        public Transform backAtgeir;

        public bool showHelmet;
        public bool showCape;
        
        [NonSerialized]
        public List<ValheimAvatarParameter> animatorParameters;

        // unfortunately, BepInEx plugin structs & classes do not work properly with Unity's serialization system
        // we could use https://github.com/xiaoxiao921/FixPluginTypesSerialization to fix this, or maybe JsonUtility
        // but this is a simple workaround that works fine, so we'll use it for now
        public List<string> animatorParameterNames;
        public List<ValheimAvatarParameterType> animatorParameterTypes;
        public List<float> animatorParameterDefaultValues;
        
        // public List<string> boolParameters;
        // public List<bool> boolParametersDefault;
        // public List<string> intParameters;
        // public List<int> intParametersDefault;
        // public List<string> floatParameters;
        // public List<float> floatParametersDefault;

        public string[] controlName;
        public ControlType[] controlTypes;
        public string[] controlParameterNames;
        public float[] controlValues;

        private void Awake()
        {
            Validate();
        }

        public void Validate()
        {
            
            // if (boolParametersDefault.Count != boolParameters.Count)
            //     boolParametersDefault.Resize(boolParameters.Count);
            //
            // if (intParametersDefault.Count != intParameters.Count)
            //     intParametersDefault.Resize(intParameters.Count);
            //
            // if (floatParametersDefault.Count != floatParameters.Count)
            //     floatParametersDefault.Resize(floatParameters.Count);

            if (controlTypes.Length != controlName.Length)
                Array.Resize(ref controlTypes, controlName.Length);

            if (controlParameterNames.Length != controlName.Length)
                Array.Resize(ref controlParameterNames, controlName.Length);

            if (controlValues.Length != controlName.Length)
                Array.Resize(ref controlValues, controlName.Length);
        }

        public void OnBeforeSerialize() {
            animatorParameterNames = [];
            animatorParameterTypes = [];
            animatorParameterDefaultValues = [];
            foreach (var parameter in animatorParameters)
            {
                animatorParameterNames.Add(parameter.name);
                animatorParameterTypes.Add(parameter.type);
                animatorParameterDefaultValues.Add(parameter.defaultValue);
            }
        }
        public void OnAfterDeserialize() {
            animatorParameters = [];
            for (var i = 0; i < animatorParameterNames.Count; i++)
            {
                animatorParameters.Add(new ValheimAvatarParameter
                {
                    name = animatorParameterNames[i],
                    type = animatorParameterTypes[i],
                    defaultValue = animatorParameterDefaultValues[i]
                });
            }
        }
    }
}