using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float kickBackBase = -0.5f;  // Wert verringert für sanfteren Start
    [SerializeField] private float kickBackVariable = 0.2f;  // Wert verringert
    [SerializeField] private Transform recoilFollowPos;
    [SerializeField] private float kickSideBase = 0.05f;  // Wert verringert
    [SerializeField] private float kickSideVariable = 0.025f;  // Wert verringert
    [SerializeField] private float kickRotateBase = -1f;  // Wert verringert
    [SerializeField] private float kickRotateVariable = 0.5f;  // Wert verringert

    [Header("Recoil Return EnemyAnim_Speed")]
    [SerializeField] private float kickBackSpeed = 10f;
    [SerializeField] private float returnSpeed = 20f;
    [SerializeField] private float recoilDecreaseRate = 0.5f;  // Definieren des Wertes für die Verringerung des Recoil
    [SerializeField] private float recoilCap = 3.0f;  // Obergrenze für den recoilMultiplier

    private float _currentRecoilPosition, _finalRecoilPosition;
    private float _currentRecoilSide, _finalRecoilSide;
    private float _currentRecoilRotation, _finalRecoilRotation;
    
    private float recoilMultiplier = 1f;

    private void Update()
    {
        HandleWeaponRecoil();
        DecreaseRecoilOverTime();
    }

    private void HandleWeaponRecoil()
    {
        _currentRecoilPosition = Mathf.Lerp(_currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
        _finalRecoilPosition = Mathf.SmoothStep(_finalRecoilPosition, _currentRecoilPosition, kickBackSpeed * Time.deltaTime);

        _currentRecoilSide = Mathf.Lerp(_currentRecoilSide, 0, returnSpeed * Time.deltaTime);
        _finalRecoilSide = Mathf.SmoothStep(_finalRecoilSide, _currentRecoilSide, kickBackSpeed * Time.deltaTime);

        _currentRecoilRotation = Mathf.Lerp(_currentRecoilRotation, 0, returnSpeed * Time.deltaTime);
        _finalRecoilRotation = Mathf.SmoothStep(_finalRecoilRotation, _currentRecoilRotation, kickBackSpeed * Time.deltaTime);

        recoilFollowPos.localPosition = new Vector3(_finalRecoilSide, 0, _finalRecoilPosition);
        recoilFollowPos.localRotation = Quaternion.Euler(0, 0, _finalRecoilRotation);
    }

    public void TriggerRecoil(float chargeValue = 1.0f)
    {
        _currentRecoilPosition = 0;
        _currentRecoilSide = 0;
        _currentRecoilRotation = 0;

        recoilMultiplier += 0.1f;
        recoilMultiplier = Mathf.Clamp(recoilMultiplier, 1, recoilCap);

        _currentRecoilPosition += (kickBackBase + Random.Range(-kickBackVariable, kickBackVariable)) * chargeValue * recoilMultiplier;
        _currentRecoilSide += (kickSideBase + Random.Range(-kickSideVariable, kickSideVariable)) * chargeValue * recoilMultiplier;
        _currentRecoilRotation += (kickRotateBase + Random.Range(-kickRotateVariable, kickRotateVariable)) * chargeValue * recoilMultiplier;
    }

    private void DecreaseRecoilOverTime()
    {
        recoilMultiplier = Mathf.Clamp(recoilMultiplier - (recoilDecreaseRate * Time.deltaTime), 1, recoilCap); 
    }

}
