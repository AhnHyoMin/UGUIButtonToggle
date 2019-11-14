using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// 버튼을 토글처럼 사용하기 위해 만듦
/// </summary>
public class UGUIExButtonToggle : Selectable, IPointerClickHandler, ISubmitHandler
{
    public enum ToggleTransition
    {
        /// <summary>
        /// Show / hide the toggle instantly
        /// </summary>
        None,

        /// <summary>
        /// Fade the toggle in / out smoothly.
        /// </summary>
        Fade
    }


    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    protected UGUIExButtonToggle()
    { }


    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();


    [SerializeField]
    private bool m_IsOn;

    [SerializeField]
    private ToggleTransition m_ToggleTransition = ToggleTransition.Fade;

    [SerializeField]
    private Image m_SelectImage = null;

    [SerializeField]
    private UGUIExButtonToggleGroup m_Group = null;

    #region Property
    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    public bool IsOn { get => m_IsOn; set => Set(value); }
    public UGUIExButtonToggleGroup Group
    {
        get => m_Group;
        set
        {
            m_Group = value;
            SetButtonGroup();
            PlayEffect(true);
        }
    }
    public Image SelectImage { get => m_SelectImage; set => m_SelectImage = value; }
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtonGroup();
        PlayEffect(true);

        // 그리드 레이아웃 그룹에서 지정한 사이즈에따라 세팅되게끔 한다

        ReSize(transform.parent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SetButtonGroup();
    }

    protected override void OnDidApplyAnimationProperties()
    {
        // Check if isOn has been changed by the animation.
        // Unfortunately there is no way to check if we don�t have a graphic.
        if (m_SelectImage != null)
        {
            bool oldValue = !Mathf.Approximately(m_SelectImage.canvasRenderer.GetColor().a, 0);
            if (m_IsOn != oldValue)
            {
                m_IsOn = oldValue;
                Set(!oldValue);
            }
        }

        base.OnDidApplyAnimationProperties();
    }

    public void SetIsOnWithoutNotify(bool value)
    {
        Set(value, false);
    }

    void Set(bool value, bool sendCallback = true)
    {
        if (m_IsOn == value)
            return;

        // if we are in a group and set to true, do group logic
        m_IsOn = value;
        if (m_Group != null && IsActive())
        {
            if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.AllowSwitchOff))
            {
                m_IsOn = true;
                m_Group.NotifyToggleOn(this, sendCallback);
            }
        }

        // Always send event when toggle is clicked, even if value didn't change
        // due to already active toggle in a toggle group being clicked.
        // Controls like Dropdown rely on this.
        // It's up to the user to ignore a selection being set to the same value it already was, if desired.
        PlayEffect(m_ToggleTransition == ToggleTransition.None);
        //if (sendCallback)
        //{
        //    UISystemProfilerApi.AddMarker("Toggle.value", this);
        //    onValueChanged.Invoke(m_IsOn);
        //}
    }


    public void ReSize(Transform _Parent)
    {
        GridLayoutGroup _LayoutGroup = null;
        if (_Parent != null)
            _LayoutGroup = _Parent.GetComponent<GridLayoutGroup>();


        if (_LayoutGroup != null)
        {
            if (targetGraphic != null)
            {
                targetGraphic.rectTransform.sizeDelta = new Vector2(_LayoutGroup.cellSize.x, _LayoutGroup.cellSize.y);

            }

            if (m_SelectImage != null)
            {
                m_SelectImage.rectTransform.sizeDelta = new Vector2(_LayoutGroup.cellSize.x, _LayoutGroup.cellSize.y);
            }
        }

    }

    public void ReSize()
    {
        GridLayoutGroup _LayoutGroup = this.transform.parent.GetComponent<GridLayoutGroup>();


        if (_LayoutGroup != null)
        {
            if (targetGraphic != null)
            {

                targetGraphic.rectTransform.sizeDelta = new Vector2(_LayoutGroup.cellSize.x, _LayoutGroup.cellSize.y);
            }

            if (m_SelectImage != null)
            {
                m_SelectImage.rectTransform.sizeDelta = new Vector2(_LayoutGroup.cellSize.x, _LayoutGroup.cellSize.y);
            }
        }

    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        IsOn = !IsOn;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Press();

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }
    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }


    private void PlayEffect(bool instant)
    {
        if (m_SelectImage == null)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            m_SelectImage.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
        else
#endif
            m_SelectImage.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
    }

    private void SetButtonGroup()
    {
        if (m_Group != null)
        {
            m_Group.UnRegisterButton(this);

            if (IsActive())
                m_Group.RegisterButton(this);


            if (m_IsOn && IsActive())
                m_Group.NotifyToggleOn(this);

        }
    }

}
