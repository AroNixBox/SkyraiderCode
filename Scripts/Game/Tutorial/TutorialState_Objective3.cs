using Extensions;
using FSM;
using TMPro;
using UnityEngine;

public class TutorialState_Objective3 : IState
{
    private readonly string _hintGoal;
    private readonly Animator _hintAnimator;
    private readonly TextMeshProUGUI _hintTMP;
    private readonly AudioSource _audioSource;
    private readonly AudioClip _objectiveCompleteSfx;
    private readonly AudioClip _newHintSfx;
    private readonly Animator _gateAnimator;
    private readonly string _animationName;
    private readonly Color _gizmoColor;
    private readonly TutorialReferences _tutorialReferences;
    public TutorialState_Objective3(TutorialReferences references, string objectiveGoal, string animationName, Color gizmoColor)
    {
        _tutorialReferences = references;
        _hintAnimator = references.hintAnimator;
        _hintTMP = references.hintTMP;
        _audioSource = references.AudioSource;
        _newHintSfx = references.newHintSfx;
        _objectiveCompleteSfx = references.objectiveCompleteSfx;
        _gateAnimator = references.gateAnimator;
        _hintGoal = objectiveGoal;
        _animationName = animationName;
        _gizmoColor = gizmoColor;
    }
    public void OnEnter()
    {
        _tutorialReferences.PlayerMeeleController.isInTutorial = false;
        
        TimerUtility.SetTimer(_tutorialReferences,  () =>
        {
            _hintTMP.text = _hintGoal;
            _hintAnimator.SetTrigger(GlobalAnimationHashes.UI_ShowHintAnim);
            _audioSource.PlayOneShot(_newHintSfx);
        },3f);
    }
    
    public void Tick()
    {
        
    }
    
    public void OnExit()
    {
        _hintAnimator.SetTrigger(GlobalAnimationHashes.UI_HideHintAnim);
        _audioSource.PlayOneShot(_objectiveCompleteSfx);
        _gateAnimator.SetTrigger(_animationName);
    }
    
    public Color GizmoState()
    {
        return _gizmoColor;
    }
}
