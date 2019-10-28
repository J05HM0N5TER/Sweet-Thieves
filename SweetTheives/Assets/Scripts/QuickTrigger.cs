//Quick Scripts by Jack Wilson, Wanderlight Games 2017.
//Thank you for purchasing this product.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuickScripts
{

    // This class has an Editor script which overrides OnInspectorGUI(). See QSEditor_QuickTrigger.
    [AddComponentMenu("Quick Scripts/Quick Trigger")]
    [RequireComponent(typeof(Collider))]

    public class QuickTrigger : MonoBehaviour
    {


        [Header("Set Interactable Tags Here")]
        public List<string> interactableTags = new List<string>();

        [Serializable]
        private class TriggerEvent : UnityEvent { }
        [SerializeField]
        private TriggerEvent TriggerEnter = new TriggerEvent();
        [SerializeField]
        private TriggerEvent TriggerStay = new TriggerEvent();
        [SerializeField]
        private TriggerEvent TriggerExit = new TriggerEvent();
        public bool triggerOnce;
        public enum inputEnum
        {
            onInputDown,
            whileInputDown,
            onInputRelease
        }
        [Space(10)]

        [Header("Optional Input Requirements for Trigger Stay")]
        public inputEnum inputType;
        public enum requireEnum
        {
            OR,
            AND
        }
        public requireEnum inputCombinationMode;
        public bool requireKeyInput;
        public string inputKeyName;
        public bool requireAxisInput;
        public string inputAxisName;
        public bool requireMouseInput;
        public int inputMouseButton;



        //bool deactivateOnEnter;
        bool deactivateOnExit;
        bool triggerIsActive = true;
        int enterEvents;
        int stayEvents;
        int exitEvents;

        void Start()
        {

            enterEvents = TriggerEventCount(TriggerEnter);
            stayEvents = TriggerEventCount(TriggerStay);
            exitEvents = TriggerEventCount(TriggerExit);

            if (interactableTags.Count == 0)
            {
                Debug.LogError(gameObject.name + " | Quick Trigger has no interactable tags. It will never be activated!", this.gameObject);
            }

            if (triggerOnce)
            {
                //triggers once but has no stay or exit event, therefore must deactivate on entry
                if (stayEvents == 0 && exitEvents == 0)
                {
                    deactivateOnExit = false;
                }
                //			else if (stayEvents > 0 || exitEvents > 0) // triggers once and has stay or exit event, therefore must deactivate on exit
                //			{
                deactivateOnExit = true;
                //			}
            }

            if (GetComponent<Collider>().isTrigger == false)
                Debug.LogError(gameObject.name + " | Quick Trigger's associated Collider is not a Trigger! Please check if 'Is Trigger' is ticked on the Collider component.", this.gameObject);
            else if (GetComponent<Collider>() == null)
                Debug.LogError(gameObject.name + " | Quick Trigger has no Collider! It needs a Collider with 'Is Trigger' ticked in order to work.", this.gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            if (triggerIsActive && interactableTags.Contains(other.tag))
            {
                if (enterEvents > 0)
                {
                    TriggerEnter.Invoke();

                    if (triggerOnce && !deactivateOnExit)
                        DeActivate();
                }
            }
        }

        void OnTriggerStay(Collider other) // Too many if statements?
        {
            if (triggerIsActive && interactableTags.Contains(other.tag))
            {
                if (InputCheck())
                {
                    if (stayEvents > 0)
                    {
                        TriggerStay.Invoke();
                    }
                }
                else if (!requireAxisInput && !requireMouseInput && !requireKeyInput) // Just do it
                {
                    if (stayEvents > 0)
                    {
                        TriggerStay.Invoke();
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (triggerIsActive && interactableTags.Contains(other.tag))
            {
                if (exitEvents > 0)
                {
                    TriggerExit.Invoke();
                }

                if (triggerOnce && deactivateOnExit)
                    DeActivate();
            }
        }


        bool InputCheck()
        {
            if (inputCombinationMode == requireEnum.AND)
            {
                // I'm so sorry it has to be like this...
                // Button, Key and Mouse
                if (requireAxisInput && requireKeyInput && requireMouseInput)
                {
                    if (CheckAxisInput() && CheckKeyInput() && CheckMouseInput())
                        return true;
                }
                // Button and Mouse
                else if (requireAxisInput && requireMouseInput)
                {
                    if (CheckAxisInput() && CheckMouseInput())
                        return true;
                }
                // Button and Key
                else if (requireAxisInput && requireKeyInput)
                {
                    if (CheckAxisInput() && CheckKeyInput())
                        return true;
                }
                // Key and Mouse
                else if (requireKeyInput && requireMouseInput)
                {
                    if (CheckKeyInput() && CheckMouseInput())
                        return true;
                }
                // Button Only
                else if (requireAxisInput)
                {
                    return CheckAxisInput();
                }
                // Key Only
                else if (requireKeyInput)
                {
                    return CheckKeyInput();
                }  // Mouse Only
                else if (requireMouseInput)
                {
                    return CheckMouseInput();
                }
                else
                    return false;
            }
            else if (inputCombinationMode == requireEnum.OR)
            {
                return InputTypeSwitch();
            }
            return false;
        }

        bool InputTypeSwitch()
        {
            switch (inputType)
            {
                // Help me
                case inputEnum.onInputDown:
                    if (requireAxisInput && Input.GetButtonDown(inputAxisName))
                    {
                        return true;
                    }
                    else if (requireMouseInput && Input.GetMouseButtonDown(inputMouseButton))
                    {
                        return true;
                    }
                    else if (requireKeyInput && Input.GetKeyDown(inputKeyName))
                    {
                        return true;
                    }
                    else
                        return false;

                case inputEnum.whileInputDown:
                    if (requireAxisInput && Input.GetButton(inputAxisName))
                    {
                        return true;
                    }
                    else if (requireMouseInput && Input.GetMouseButton(inputMouseButton))
                    {
                        return true;
                    }
                    else if (requireKeyInput && Input.GetKey(inputKeyName))
                    {
                        return true;
                    }
                    else
                        return false;
                case inputEnum.onInputRelease:
                    if (requireAxisInput && Input.GetButtonUp(inputAxisName))
                    {
                        return true;
                    }
                    else if (requireMouseInput && Input.GetMouseButtonUp(inputMouseButton))
                    {
                        return true;
                    }
                    else if (requireKeyInput && Input.GetKeyUp(inputKeyName))
                    {
                        return true;
                    }
                    else
                        return false;
            }
            return false;
        }

        bool CheckAxisInput()
        {
            switch (inputType)
            {
                case inputEnum.onInputDown:
                    if (requireAxisInput && Input.GetButtonDown(inputAxisName))
                        return true;
                    else
                        return false;
                case inputEnum.whileInputDown:
                    if (requireAxisInput && Input.GetButton(inputAxisName))
                        return true;
                    else
                        return false;
                case inputEnum.onInputRelease:
                    if (requireAxisInput && Input.GetButtonUp(inputAxisName))
                        return true;
                    else
                        return false;
            }
            return false;
        }

        bool CheckKeyInput()
        {
            switch (inputType)
            {
                case inputEnum.onInputDown:
                    if (requireKeyInput && Input.GetKeyDown(inputKeyName))
                        return true;
                    else
                        return false;
                case inputEnum.whileInputDown:
                    if (requireKeyInput && Input.GetKey(inputKeyName))
                        return true;
                    else
                        return false;
                case inputEnum.onInputRelease:
                    if (requireKeyInput && Input.GetKeyUp(inputKeyName))
                        return true;
                    else
                        return false;
            }
            return false;
        }

        bool CheckMouseInput()
        {
            switch (inputType)
            {
                case inputEnum.onInputDown:
                    if (requireMouseInput && Input.GetMouseButtonDown(inputMouseButton))
                        return true;
                    else
                        return false;
                case inputEnum.whileInputDown:
                    if (requireMouseInput && Input.GetMouseButton(inputMouseButton))
                        return true;
                    else
                        return false;
                case inputEnum.onInputRelease:
                    if (requireMouseInput && Input.GetMouseButtonUp(inputMouseButton))
                        return true;
                    else
                        return false;
            }
            return false;
        }



        int TriggerEventCount(UnityEvent action)
        {
            return action.GetPersistentEventCount();
        }

        public void AddTag(string tag)
        {
            if (!interactableTags.Contains(tag))
                interactableTags.Add(tag);
            else
                Debug.LogWarning(this.name + " | Quick Trigger already has the " + tag + " tag assigned");
        }

        public void RemoveTag(string tag)
        {
            if (tag != null)
                interactableTags.Remove(tag);
        }

        public void RemoveLast()
        {
            interactableTags.RemoveAt(interactableTags.Count - 1);
        }

        void DeActivate()
        {
            triggerIsActive = false;
        }

        #region Public Events
        public void SetTriggerOnce(bool b)
        {
            triggerOnce = b;
        }
        public void DeActivateTrigger()
        {
            DeActivate();
        }
        public void RequireKeyInput(bool b)
        {
            requireKeyInput = b;
        }
        public void RequireMouseInput(bool b)
        {
            requireMouseInput = b;
        }
        public void RequireAxisInput(bool b)
        {
            requireAxisInput = b;
        }
        public void SetKeyInputName(string s)
        {
            inputKeyName = s;
        }
        public void SetAxisInputName(string s)
        {
            inputAxisName = s;
        }
        public void SetMouseInputButton(int i)
        {
            if (i < 3)
                inputMouseButton = i;
            else
                Debug.LogError("You are trying to manually change Mouse Input Button on " + this.name + ". Integer value must be 0 - 2.");
        }
        public void SetInputType(int i)
        {
            if (i < 3)
            {
                inputEnum e = (inputEnum)i;
                inputType = e;
            }
            else
                Debug.LogError("You are trying to manually change Input Type on " + this.name + ". Integer value must be 0 - 2.");
        }
        public void SetInputCombination(int i)
        {
            if (i < 2)
            {
                requireEnum e = (requireEnum)i;
                inputCombinationMode = e;
            }
            else
                Debug.LogError("You are trying to manually change Input Combination Type on " + this.name + ". Integer value must be 0 - 1.");
        }
        #endregion
    }
}
