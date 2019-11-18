//Quick Scripts by Jack Wilson, Wanderlight Games 2017.
//Thank you for purchasing this product.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace QuickScripts
{

    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Quick Scripts/Quick Light")]
    public class QuickLight : MonoBehaviour
    {

        private Light myLight;

        public enum lightAnimation
        {
            standard,
            pulseSlow,
            pulseFast,
            strobe,
            flicker1,
            flicker2,
            flicker3,
            candle,
            fire,
            television,
            storm,
            custom
        }
        public lightAnimation lightType;
        [Range(0, 8)]
        private float localIntensity;
        public float speed = 2;
        public AnimationCurve lightOverTime;
        public bool overrideColor;
        public Gradient colorGradient;
        private float t;

        Dictionary<float, float> animationKeyFrames = new Dictionary<float, float>();
        private lightAnimation prevLightType;


        void Start()
        {
            myLight = GetComponent<Light>();
            localIntensity = myLight.intensity;
            GetAnimationType();
            lightOverTime = GetComponent<QuickLight>().lightOverTime;
        }

        void GetAnimationType()
        {
            animationKeyFrames.Clear();
            switch (lightType)
            {
                case lightAnimation.standard:
                    SetKeyFrames_Standard();
                    break;
                case lightAnimation.pulseSlow:
                    SetKeyFrames_PulseSlow();
                    break;
                case lightAnimation.pulseFast:
                    SetKeyFrames_PulseFast();
                    break;
                case lightAnimation.flicker1:
                    SetKeyFrames_Flicker1();
                    break;
                case lightAnimation.flicker2:
                    SetKeyFrames_Flicker2();
                    break;
                case lightAnimation.flicker3:
                    SetKeyFrames_Flicker3();
                    break;
                case lightAnimation.strobe:
                    SetKeyFrames_Strobe();
                    break;
                case lightAnimation.candle:
                    SetKeyFrames_Candle();
                    break;
                case lightAnimation.fire:
                    SetKeyFrames_Fire();
                    break;
                case lightAnimation.television:
                    SetKeyFrames_TV();
                    break;
                case lightAnimation.storm:
                    SetKeyFrames_Storm();
                    break;
            }
            prevLightType = lightType;
        }

        // Update is called once per frame
        void Update()
        {

            if (lightType != lightAnimation.standard)
                myLight.intensity = lightOverTime.Evaluate(Time.time * (speed / 2));

            if (prevLightType != lightType)
                GetAnimationType();

        }

        void FixedUpdate()
        {
            if (overrideColor)
                UpdateColorGradient();
        }

        void UpdateColorGradient() // Called from Update
        {
            t += Time.fixedDeltaTime * speed / 2;
            myLight.color = colorGradient.Evaluate(t);

            if (t > 1)
                t = 0;
        }

        void SetKeyFrames_Standard()
        {
            animationKeyFrames.Add(0, 1 * localIntensity);
            ApplyAnimation(WrapMode.Once);
        }
        void SetKeyFrames_PulseSlow()
        {
            animationKeyFrames.Add(0, 0 * localIntensity);
            animationKeyFrames.Add(1, 1 * localIntensity);
            animationKeyFrames.Add(2, 0 * localIntensity);
            ApplyAnimation(WrapMode.Loop);
        }
        void SetKeyFrames_PulseFast()
        {
            animationKeyFrames.Add(0, 0 * localIntensity);
            animationKeyFrames.Add(0.25f, 1 * localIntensity);
            ApplyAnimation(WrapMode.PingPong);
        }
        void SetKeyFrames_Flicker1()
        {
            animationKeyFrames.Add(0, 0.6f * localIntensity);
            animationKeyFrames.Add(0.05f, 1f * localIntensity);
            animationKeyFrames.Add(0.1f, 0.6f * localIntensity);
            animationKeyFrames.Add(0.15f, 1f * localIntensity);
            animationKeyFrames.Add(0.2f, 0.6f * localIntensity);

            animationKeyFrames.Add(0.55f, 0.6f * localIntensity);
            animationKeyFrames.Add(0.6f, 1f * localIntensity);
            animationKeyFrames.Add(0.65f, 0.6f * localIntensity);


            animationKeyFrames.Add(1f, 0.6f * localIntensity);
            animationKeyFrames.Add(1.05f, 1f * localIntensity);
            animationKeyFrames.Add(1.1f, 0.6f * localIntensity);

            animationKeyFrames.Add(1.6f, 0.6f * localIntensity);
            animationKeyFrames.Add(1.65f, 1f * localIntensity);
            animationKeyFrames.Add(1.7f, 0.6f * localIntensity);
            animationKeyFrames.Add(1.75f, 1f * localIntensity);
            animationKeyFrames.Add(1.8f, 0.6f * localIntensity);

            animationKeyFrames.Add(2.35f, 0.6f * localIntensity);
            animationKeyFrames.Add(2.4f, 1f * localIntensity);
            animationKeyFrames.Add(2.45f, 0.6f * localIntensity);

            animationKeyFrames.Add(3.2f, 0.6f * localIntensity);
            animationKeyFrames.Add(3.25f, 1f * localIntensity);
            animationKeyFrames.Add(3.3f, 0.6f * localIntensity);
            animationKeyFrames.Add(3.35f, 1f * localIntensity);
            animationKeyFrames.Add(3.4f, 0.6f * localIntensity);

            animationKeyFrames.Add(4f, 0.6f * localIntensity);
            animationKeyFrames.Add(4.05f, 1f * localIntensity);
            animationKeyFrames.Add(4.1f, 0.6f * localIntensity);

            animationKeyFrames.Add(6f, 0.6f * localIntensity);
            animationKeyFrames.Add(6.05f, 1f * localIntensity);
            animationKeyFrames.Add(6.1f, 0.6f * localIntensity);

            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();
        }

        void SetKeyFrames_Flicker2()
        {
            animationKeyFrames.Add(0, 0f * localIntensity);
            animationKeyFrames.Add(0.05f, 1f * localIntensity);
            animationKeyFrames.Add(0.1f, 0f * localIntensity);
            animationKeyFrames.Add(0.15f, 1f * localIntensity);
            animationKeyFrames.Add(0.2f, 0.6f * localIntensity);

            animationKeyFrames.Add(0.55f, 0f * localIntensity);
            animationKeyFrames.Add(0.6f, 1f * localIntensity);
            animationKeyFrames.Add(0.65f, 0f * localIntensity);


            animationKeyFrames.Add(1f, 0f * localIntensity);
            animationKeyFrames.Add(1.05f, 1f * localIntensity);
            animationKeyFrames.Add(1.1f, 0f * localIntensity);

            animationKeyFrames.Add(1.6f, 0f * localIntensity);
            animationKeyFrames.Add(1.65f, 1f * localIntensity);
            animationKeyFrames.Add(1.7f, 0f * localIntensity);
            animationKeyFrames.Add(1.75f, 1f * localIntensity);
            animationKeyFrames.Add(1.8f, 0f * localIntensity);

            animationKeyFrames.Add(2.35f, 0f * localIntensity);
            animationKeyFrames.Add(2.4f, 1f * localIntensity);
            animationKeyFrames.Add(2.45f, 0f * localIntensity);

            animationKeyFrames.Add(3.2f, 0f * localIntensity);
            animationKeyFrames.Add(3.25f, 1f * localIntensity);
            animationKeyFrames.Add(3.3f, 0f * localIntensity);
            animationKeyFrames.Add(3.35f, 1f * localIntensity);
            animationKeyFrames.Add(3.4f, 0f * localIntensity);

            animationKeyFrames.Add(4f, 0f * localIntensity);
            animationKeyFrames.Add(4.05f, 1f * localIntensity);
            animationKeyFrames.Add(4.1f, 0f * localIntensity);

            animationKeyFrames.Add(6f, 0f * localIntensity);
            animationKeyFrames.Add(6.05f, 1f * localIntensity);
            animationKeyFrames.Add(6.1f, 0f * localIntensity);
            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();
        }

        void SetKeyFrames_Flicker3()
        {
            animationKeyFrames.Add(0.05f, 0.3f * localIntensity);
            animationKeyFrames.Add(0.1f, 1f * localIntensity);
            animationKeyFrames.Add(0.15f, 0.3f * localIntensity);
            animationKeyFrames.Add(0.2f, 1f * localIntensity);
            animationKeyFrames.Add(0.25f, 0.3f * localIntensity);
            animationKeyFrames.Add(0.3f, 0.5f * localIntensity);
            animationKeyFrames.Add(0.35f, 0.3f * localIntensity);

            animationKeyFrames.Add(0.55f, 0.3f * localIntensity);
            animationKeyFrames.Add(0.6f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.65f, 0.3f * localIntensity);

            animationKeyFrames.Add(1.05f, 0.3f * localIntensity);
            animationKeyFrames.Add(1.1f, 1f * localIntensity);
            animationKeyFrames.Add(1.15f, 0.7f * localIntensity);

            animationKeyFrames.Add(1.55f, 0.3f * localIntensity);
            animationKeyFrames.Add(1.6f, 1f * localIntensity);
            animationKeyFrames.Add(1.65f, 0.3f * localIntensity);
            animationKeyFrames.Add(1.7f, 0.4f * localIntensity);
            animationKeyFrames.Add(1.75f, 0.3f * localIntensity);

            animationKeyFrames.Add(2.55f, 0.3f * localIntensity);
            animationKeyFrames.Add(2.6f, 1f * localIntensity);
            animationKeyFrames.Add(2.65f, 0.3f * localIntensity);

            animationKeyFrames.Add(3.05f, 0.3f * localIntensity);
            animationKeyFrames.Add(3.1f, 0.4f * localIntensity);
            animationKeyFrames.Add(3.15f, 0.3f * localIntensity);
            animationKeyFrames.Add(3.2f, 1f * localIntensity);
            animationKeyFrames.Add(3.25f, 0.4f * localIntensity);

            animationKeyFrames.Add(3.95f, 0.3f * localIntensity);
            animationKeyFrames.Add(4.0f, 0.7f * localIntensity);
            animationKeyFrames.Add(4.05f, 0.3f * localIntensity);
            animationKeyFrames.Add(4.1f, 0.3f * localIntensity);
            animationKeyFrames.Add(4.15f, 1f * localIntensity);
            animationKeyFrames.Add(4.2f, 0.3f * localIntensity);
            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();

        }

        void SetKeyFrames_Strobe()
        {
            animationKeyFrames.Add(0, 0 * localIntensity);
            animationKeyFrames.Add(0.04f, 0 * localIntensity);
            animationKeyFrames.Add(0.05f, 1 * localIntensity);
            animationKeyFrames.Add(0.14f, 1 * localIntensity);
            animationKeyFrames.Add(0.15f, 0 * localIntensity);
            animationKeyFrames.Add(0.2f, 0 * localIntensity);
            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();
        }
        void SetKeyFrames_Candle()
        {
            animationKeyFrames.Add(0, 0.9f * localIntensity);
            animationKeyFrames.Add(0.5f, 0.95f * localIntensity);
            animationKeyFrames.Add(1, 0.9f * localIntensity);
            animationKeyFrames.Add(2, 0.9f * localIntensity);
            animationKeyFrames.Add(2.8f, 0.85f * localIntensity);
            animationKeyFrames.Add(3.05f, 0.9f * localIntensity);
            animationKeyFrames.Add(3.1f, 0.95f * localIntensity);
            animationKeyFrames.Add(3.15f, 0.9f * localIntensity);
            animationKeyFrames.Add(3.2f, 0.95f * localIntensity);
            animationKeyFrames.Add(3.25f, 0.9f * localIntensity);
            animationKeyFrames.Add(3.3f, 0.95f * localIntensity);
            animationKeyFrames.Add(3.35f, 0.9f * localIntensity);
            animationKeyFrames.Add(4f, 0.9f * localIntensity);
            animationKeyFrames.Add(5f, 0.95f * localIntensity);
            animationKeyFrames.Add(6f, 0.9f * localIntensity);
            animationKeyFrames.Add(6.5f, 1 * localIntensity);
            animationKeyFrames.Add(7f, 0.95f * localIntensity);
            animationKeyFrames.Add(8f, 0.9f * localIntensity);
            animationKeyFrames.Add(8.05f, 0.9f * localIntensity);
            animationKeyFrames.Add(8.1f, 0.95f * localIntensity);
            animationKeyFrames.Add(8.15f, 0.9f * localIntensity);
            animationKeyFrames.Add(8.2f, 0.95f * localIntensity);
            animationKeyFrames.Add(8.25f, 0.9f * localIntensity);
            animationKeyFrames.Add(8.3f, 0.95f * localIntensity);
            animationKeyFrames.Add(8.35f, 0.9f * localIntensity);
            animationKeyFrames.Add(10f, 0.9f * localIntensity);

            ApplyAnimation(WrapMode.Loop);
        }

        void SetKeyFrames_Fire()
        {
            animationKeyFrames.Add(0, 0.8f * localIntensity);
            animationKeyFrames.Add(0.05f, 0.9f * localIntensity);
            animationKeyFrames.Add(0.1f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.15f, 1f * localIntensity);
            animationKeyFrames.Add(0.2f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.25f, 0.9f * localIntensity);
            animationKeyFrames.Add(0.3f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.35f, 0.7f * localIntensity);
            animationKeyFrames.Add(0.4f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.45f, 1f * localIntensity);
            animationKeyFrames.Add(0.5f, 0.7f * localIntensity);
            animationKeyFrames.Add(0.55f, 0.9f * localIntensity);
            animationKeyFrames.Add(0.65f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.7f, 1f * localIntensity);
            animationKeyFrames.Add(0.75f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.8f, 0.9f * localIntensity);
            animationKeyFrames.Add(0.85f, 0.7f * localIntensity);
            animationKeyFrames.Add(0.9f, 0.8f * localIntensity);
            animationKeyFrames.Add(0.95f, 0.7f * localIntensity);

            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();
        }

        void SetKeyFrames_TV()
        {
            animationKeyFrames.Add(0, 1f * localIntensity);
            animationKeyFrames.Add(1, 0.6f * localIntensity);
            animationKeyFrames.Add(1.1f, 0.8f * localIntensity);
            animationKeyFrames.Add(2.2f, 0.7f * localIntensity);
            animationKeyFrames.Add(2.3f, 0.5f * localIntensity);
            animationKeyFrames.Add(2.5f, 0.4f * localIntensity);
            animationKeyFrames.Add(3.5f, 0.6f * localIntensity);
            animationKeyFrames.Add(3.6f, 1f * localIntensity);
            animationKeyFrames.Add(4f, 0.8f * localIntensity);
            animationKeyFrames.Add(4.1f, 0.7f * localIntensity);
            animationKeyFrames.Add(5f, 0.7f * localIntensity);
            animationKeyFrames.Add(5.1f, 1f * localIntensity);

            ApplyAnimation(WrapMode.Loop);
            SetTangentsToLinear();
        }

        void SetKeyFrames_Storm()
        {
            animationKeyFrames.Add(0, 0f * localIntensity);
            animationKeyFrames.Add(0.01f, 0.6f * localIntensity);
            animationKeyFrames.Add(0.05f, 0f * localIntensity);
            animationKeyFrames.Add(0.1f, 0f * localIntensity);
            animationKeyFrames.Add(0.11f, 0.4f * localIntensity);
            animationKeyFrames.Add(0.15f, 0f * localIntensity);

            animationKeyFrames.Add(1, 0f * localIntensity);
            animationKeyFrames.Add(1.01f, 0.6f * localIntensity);
            animationKeyFrames.Add(1.05f, 0f * localIntensity);
            animationKeyFrames.Add(1.1f, 0f * localIntensity);
            animationKeyFrames.Add(1.11f, 0.5f * localIntensity);
            animationKeyFrames.Add(1.15f, 0f * localIntensity);

            animationKeyFrames.Add(2.15f, 0f * localIntensity);
            animationKeyFrames.Add(2.16f, 1f * localIntensity);
            animationKeyFrames.Add(2.2f, 1f * localIntensity);
            animationKeyFrames.Add(2.3f, 0f * localIntensity);


            animationKeyFrames.Add(3, 0f * localIntensity);
            animationKeyFrames.Add(3.01f, 1f * localIntensity);
            animationKeyFrames.Add(3.05f, 0f * localIntensity);
            animationKeyFrames.Add(3.06f, 0.4f * localIntensity);
            animationKeyFrames.Add(3.1f, 0f * localIntensity);

            animationKeyFrames.Add(4, 0f * localIntensity);
            animationKeyFrames.Add(4.01f, 0.6f * localIntensity);
            animationKeyFrames.Add(4.05f, 0f * localIntensity);
            animationKeyFrames.Add(4.2f, 0f * localIntensity);
            animationKeyFrames.Add(4.21f, 0.5f * localIntensity);
            animationKeyFrames.Add(4.25f, 0f * localIntensity);

            animationKeyFrames.Add(6, 0f * localIntensity);
            animationKeyFrames.Add(6.01f, 0.6f * localIntensity);
            animationKeyFrames.Add(6.05f, 0f * localIntensity);
            animationKeyFrames.Add(6.1f, 0f * localIntensity);
            animationKeyFrames.Add(6.11f, 1f * localIntensity);
            animationKeyFrames.Add(6.5f, 1f * localIntensity);
            animationKeyFrames.Add(6.55f, 0f * localIntensity);


            ApplyAnimation(WrapMode.Loop);
            //SetTangentsToLinear ();

        }

        void ApplyAnimation(WrapMode wrapmode)
        {
            for (int i = lightOverTime.keys.Length; i > 0; i--)
            {
                lightOverTime.RemoveKey(0);
            }

            foreach (var key in animationKeyFrames)
            {
                lightOverTime.AddKey(key.Key, key.Value);
            }
            lightOverTime.postWrapMode = wrapmode;
        }

        void SetTangentsToLinear()
        {
            for (int i = 0; i < lightOverTime.keys.Length; ++i)
            {
                float inTanget = 0;
                float outTangent = 0;
                bool setInTangent = false;
                bool setOutTangent = false;
                Vector2 prevKeyPos;
                Vector2 currentKeyPos;
                Vector2 nextKeyPos;
                Vector2 differencePos;
                Keyframe prevKey;
                Keyframe nextKey;
                Keyframe currentKey = lightOverTime[i];

                if (i - 1 >= 0)
                {
                    prevKey = lightOverTime[i - 1];
                }
                else
                {
                    prevKey = lightOverTime[0];
                }

                if (i + 1 < lightOverTime.keys.Length)
                {
                    nextKey = lightOverTime[i + 1];
                }
                else
                {
                    nextKey = lightOverTime[lightOverTime.keys.Length - 1];
                }

                if (i == 0)
                {
                    inTanget = 0; setInTangent = true;
                }

                if (i == lightOverTime.keys.Length - 1)
                {
                    outTangent = 0; setOutTangent = true;
                }

                if (!setInTangent)
                {
                    prevKeyPos = new Vector2(prevKey.time, prevKey.value);
                    currentKeyPos = new Vector2(currentKey.time, currentKey.value);

                    differencePos = currentKeyPos - prevKeyPos;

                    inTanget = differencePos.y / differencePos.x;
                }
                if (!setOutTangent)
                {
                    currentKeyPos = new Vector2(currentKey.time, currentKey.value);
                    nextKeyPos = new Vector2(nextKey.time, nextKey.value);

                    differencePos = nextKeyPos - currentKeyPos;

                    outTangent = differencePos.y / differencePos.x;
                }

                currentKey.inTangent = inTanget;
                currentKey.outTangent = outTangent;
                lightOverTime.MoveKey(i, currentKey);
            }
        }

        #region Public Events
        public void SetLightType(int t)
        {
            if (t < 12)
            {
                lightAnimation e = (lightAnimation)t;
                lightType = e;
            }
            else
                Debug.LogError("You are trying to manually change Light Type on " + this.name + ". Integer value must be 0 - 11.");
        }
        public void SetSpeed(float t)
        {
            speed = t;
        }
        public void SetOverrideColorBool(bool b)
        {
            overrideColor = b;
        }
        #endregion
    }
}

// Animation Curve linear tangent code thanks to 'metroidsnes' on Unity3d forums. 
// https://forum.unity3d.com/threads/how-to-set-an-animation-curve-to-linear-through-scripting.151683/#post-1918520