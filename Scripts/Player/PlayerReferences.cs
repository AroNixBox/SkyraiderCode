using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerReferences : MonoBehaviour
    {
        public StarterAssetsInputs StarterAssetsInputs { get; private set;  }
        public ThirdPersonController ThirdPersonController { get; private set;  }
        public PlayerUI PlayerUI { get; private set;  }
        public PlayerInput PlayerInput { get; private set;  }
        public ShooterController ShooterController { get; private set;  }
        public Animator Animator { get; private set;  }
        public PlayerAnimator PlayerAnimator { get; private set;  }
        public CharacterController CharacterController { get; private set;  }

        private void Awake()
        {
            StarterAssetsInputs = GetComponent<StarterAssetsInputs>();
            ThirdPersonController = GetComponent<ThirdPersonController>();
            PlayerUI = GetComponent<PlayerUI>();
            PlayerInput = GetComponent<PlayerInput>();
            ShooterController = GetComponent<ShooterController>();
            Animator = GetComponent<Animator>();
            PlayerAnimator = GetComponent<PlayerAnimator>();
            CharacterController = GetComponent<CharacterController>();
        }
    }
}

