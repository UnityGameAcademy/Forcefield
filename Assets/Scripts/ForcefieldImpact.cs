using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForcefieldDemo
{
    public class ForcefieldImpact : MonoBehaviour
    {

        // time until impact ripple dissipates
        [Range(0.1f, 5f)]
        [SerializeField] private float dampenTime = 1.5f;

        // maximum displacement on impact
        [Range(0.002f,0.1f)]
        [SerializeField] private float impactRippleAmplitude = 0.005f;
        [Range(0.05f, 0.5f)]
        [SerializeField] private float impactRippleMaxRadius = 0.35f;

        // allow mouse clicks for testing 
        [SerializeField] private bool clickToImpact;

        //// slight delay between clicks to prevent spamming
        private const float coolDownMax = 0.25f;
        private float coolDownWindow;

        // main camera
        private Camera cam;

        // reference to this MeshRenderer
        private MeshRenderer meshRenderer;

        void Start()
        {
            if (cam == null && Camera.main != null)
            {
                cam = Camera.main;
            }

            meshRenderer = GetComponent<MeshRenderer>();

            coolDownWindow = 0;

        }

        #region DIAGNOSTIC 
        private void UpdateMouse()
        {
            coolDownWindow -= Time.deltaTime;

            if (coolDownWindow <= 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ClickToImpact();
                }
            }
        }

        // allow mouse clicks to test forcefield - useful for diagnostic
        private void ClickToImpact()
        {
            if (cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform hitXform = hit.transform;

                if (hitXform == this.transform)
                {
                    coolDownWindow = coolDownMax;

                    ApplyImpact(hit.point, hit.normal);
                }
                else
                {
                    // for debugging if we hit another object instead
                    Debug.Log("Hit " + hitXform.name);
                }
            }
        }
        #endregion

        public void EnableRipple(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableRipple", onOff);
        }

        public void EnableRimGlow(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableRimGlow", onOff);
        }

        public void EnableScanLine(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableScanLine", onOff);
        }

        public void EnableFillTexture(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableFillTexture", onOff);
        }

        public void EnableIntersection(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableIntersection", onOff);
        }


        // impact Forcefield, passing in hit point and direction
        public void ApplyImpact(Vector3 hitPoint, Vector3 rippleDirection)
        {
            if (meshRenderer != null)
            {
                EnableRipple(true);
                meshRenderer.material.SetFloat("_impactRippleMaxRadius", impactRippleMaxRadius);
                meshRenderer.material.SetFloat("_impactRippleAmplitude", impactRippleAmplitude);
                meshRenderer.material.SetVector("_impactRippleDirection", rippleDirection);
                meshRenderer.material.SetVector("_impactPoint", hitPoint);

            }
        }

        // impact Forcefield, passing in RaycastHit
        public void ApplyImpact(RaycastHit hit)
        {
            if (meshRenderer != null)
            {
                EnableRipple(true);
                meshRenderer.material.SetFloat("_impactRippleMaxRadius", impactRippleMaxRadius);
                meshRenderer.material.SetFloat("_impactRippleAmplitude", impactRippleAmplitude);
                meshRenderer.material.SetVector("_impactRippleDirection", hit.normal);
                meshRenderer.material.SetVector("_impactPoint", hit.point);

            }
        }

        // gradually slow ripple motion 
        private void Dampen()
        {
            if (meshRenderer != null)
            {
                // get the current amplitude
                float currentAmplitude = meshRenderer.material.GetFloat("_impactRippleAmplitude");

                // decrement by a small amount per frame
                float newAmplitude = currentAmplitude - (impactRippleAmplitude * Time.deltaTime / dampenTime);

                // clamp to positive values
                newAmplitude = Mathf.Clamp(newAmplitude, 0f, newAmplitude);

                // if negative, disable the ripple; otherwise, set the new amplitude
                if (newAmplitude <= 0)
                {
                    EnableRipple(false);
                }
                else
                {
                    meshRenderer.material.SetFloat("_impactRippleAmplitude", newAmplitude);
                }
            }
        }

        void Update()
        {
            if (clickToImpact)
            {
                UpdateMouse();
            }

            Dampen();
        }
    }
}