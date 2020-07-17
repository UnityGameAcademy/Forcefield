using UnityEngine;

namespace ForcefieldDemo
{
    // simple weapon script to shoot particles based on mouse click,
    // useful for testing the Forcefield Shader Graph
    public class DemoWeapon : MonoBehaviour
    {
        //// "hitscan" particles for nearly instantaneous weapon 
        [SerializeField] private ParticleSystem projectileParticles;

        // time between shots
        [SerializeField] private float fireRate;

        // one-shot audio played for each shot
        [SerializeField] private AudioClip audioClip;

        // audio source to play sound fx
        [SerializeField] private AudioSource audioSource;

        // main camera for raycasting
        [SerializeField] private Camera cam;

        // do not fire over this area of the screen
        [SerializeField] private RectTransform deadZone;

        private bool isButtonDown;
        private float timeElapsed;


        private void Awake()
        {
            //initialize Camera if unspecified
            if (cam == null)
            {
                cam = Camera.main;
            }
        }

        // update after the physics update
        private void LateUpdate()
        {
            ShootAtMousePosition();
        }


        // fire ParticleSystem weapon at cursor on mouse click
        public void ShootAtMousePosition()
        {
            if (cam == null)
            {
                Debug.Log("DEMOWEAPON ShootAtMousePosition: missing main Camera!");
                return;
            }

            if (projectileParticles == null)
            {
                Debug.Log("DEMOWEAPON ShootAtMousePosition: missing Particle Systems!");
                return;
            }

            Vector2 mousePosition = Input.mousePosition;

            // mouse in "dead zone"
            if (deadZone != null && RectTransformUtility.RectangleContainsScreenPoint(deadZone, mousePosition))
            {
                return;
            }

            // ray through mouse position
            Ray ray = cam.ScreenPointToRay(mousePosition);


            // if we do not hit any geometry, do nothing
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }

            // rotate weapon to hit point
            transform.rotation = Quaternion.LookRotation(hit.point - transform.position);

            // if mouse button is held down...
            if (Input.GetMouseButtonDown(0))
            {
                isButtonDown = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isButtonDown = false;
            }

            // track elapsed time to see if we can shoot
            timeElapsed += Time.deltaTime;

            if (!isButtonDown || timeElapsed < fireRate)
            {
                return;
            }

            // emit one particle
            if (projectileParticles != null)
            {
                projectileParticles.Emit(1);
            }

            // play one-shot sound
            if (audioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(audioClip);
            }

            // reset time elapsed
            timeElapsed = 0;
        }
    }
}