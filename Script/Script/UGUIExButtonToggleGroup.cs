using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UGUIExButtonToggleGroup : UIBehaviour
    {
        private List<UGUIExButtonToggle> m_ButtonGroup = new List<UGUIExButtonToggle>();

        [SerializeField]
        private bool m_AllowSwitchOff = false;

        public bool AllowSwitchOff { get => m_AllowSwitchOff; set => m_AllowSwitchOff = value; }

        protected override void Start()
        {
            EnsureValidState();
            base.Start();
        }

        public void GroupResize()
        {
            for (int i = 0; i < m_ButtonGroup.Count; i++)
            {
                m_ButtonGroup[i].ReSize();
            }
        }

        public void RegisterButton(UGUIExButtonToggle _UGUIExButton)
        {
            if (!m_ButtonGroup.Contains(_UGUIExButton))
                m_ButtonGroup.Add(_UGUIExButton);
        }


        public void UnRegisterButton(UGUIExButtonToggle _UGUIExButton)
        {
            if (m_ButtonGroup.Contains(_UGUIExButton))
                m_ButtonGroup.Remove(_UGUIExButton);
        }

        public void EnsureValidState()
        {
            if (!m_AllowSwitchOff && !AnyTogglesOn() && m_ButtonGroup.Count != 0)
            {
                m_ButtonGroup[0].IsOn = true;
                NotifyToggleOn(m_ButtonGroup[0]);
            }
        }

        public void NotifyToggleOn(UGUIExButtonToggle _UGUIExButton, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(_UGUIExButton);
            // disable all toggles in the group
            for (var i = 0; i < m_ButtonGroup.Count; i++)
            {
                if (m_ButtonGroup[i] == _UGUIExButton)
                    continue;

                if (sendCallback)
                    m_ButtonGroup[i].IsOn = false;
                else
                    m_ButtonGroup[i].SetIsOnWithoutNotify(false);
            }
        }


        private void ValidateToggleIsInGroup(UGUIExButtonToggle _UGUIExButtontoggle)
        {
            if (_UGUIExButtontoggle == null || !m_ButtonGroup.Contains(_UGUIExButtontoggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { _UGUIExButtontoggle, this }));
        }

        public bool AnyTogglesOn()
        {
            return m_ButtonGroup.Find(x => x.IsOn) != null;
        }

    }
}