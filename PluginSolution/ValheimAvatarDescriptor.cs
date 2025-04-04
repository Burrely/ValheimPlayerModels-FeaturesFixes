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

    public class ValheimAvatarDescriptor : MonoBehaviour
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

        public List<string> boolParameters;
        public List<bool> boolParametersDefault;
        public List<string> intParameters;
        public List<int> intParametersDefault;
        public List<string> floatParameters;
        public List<float> floatParametersDefault;

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
            if (boolParametersDefault.Count != boolParameters.Count)
                boolParametersDefault.Resize(boolParameters.Count);

            if (intParametersDefault.Count != intParameters.Count)
                intParametersDefault.Resize(intParameters.Count);

            if (floatParametersDefault.Count != floatParameters.Count)
                floatParametersDefault.Resize(floatParameters.Count);

            if (controlTypes.Length != controlName.Length)
                Array.Resize(ref controlTypes, controlName.Length);

            if (controlParameterNames.Length != controlName.Length)
                Array.Resize(ref controlParameterNames, controlName.Length);

            if (controlValues.Length != controlName.Length)
                Array.Resize(ref controlValues, controlName.Length);
        }
    }
}