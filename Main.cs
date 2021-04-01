using UnityEngine;

namespace ml_mictoggle
{
    public class MCT : MelonLoader.MelonMod
    {
        SteamVR_ControllerManager m_controllerManager = null;
        SteamVR_TrackedController m_trackedController = null;

        private const long m_toggleDelay = 5000000L;

        private bool m_enabled = true;
        private int m_hand = 0;

        private bool m_buttonOldState = false;
        private long m_lastToggleTick = 0L;

        private void FindControllerManager()
        {
            VRCInputProcessor l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Vive];
            if (l_input)
            {
                VRCInputProcessorVive l_castInput = l_input.TryCast<VRCInputProcessorVive>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }

            l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.ViveAdvanced];
            if (l_input)
            {
                VRCInputProcessorViveAdvanced l_castInput = l_input.TryCast<VRCInputProcessorViveAdvanced>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }

            l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Index];
            if (l_input)
            {
                VRCInputProcessorIndex l_castInput = l_input.TryCast<VRCInputProcessorIndex>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }
        }

        private void FindTrackedController()
        {
            switch (m_hand)
            {
                case 0:
                default:
                    m_trackedController = m_controllerManager.field_Public_GameObject_0?.GetComponent<SteamVR_TrackedController>();
                    break;
                case 1:
                    m_trackedController = m_controllerManager.field_Public_GameObject_1?.GetComponent<SteamVR_TrackedController>();
                    break;
            }
            if (!m_trackedController)
            {
                switch (m_hand)
                {
                    case 0:
                    default:
                        m_trackedController = m_controllerManager.field_Public_GameObject_0?.AddComponent<SteamVR_TrackedController>();
                        break;
                    case 1:
                        m_trackedController = m_controllerManager.field_Public_GameObject_1?.AddComponent<SteamVR_TrackedController>();
                        break;
                }
            }
        }

        public override void OnApplicationStart()
        {
            MelonLoader.MelonPreferences.CreateCategory("MCT", "Microphone fast VR toggle");
            MelonLoader.MelonPreferences.CreateEntry("MCT", "MicToggle", true, "Enable toggling");
            MelonLoader.MelonPreferences.CreateEntry("MCT", "MicHand", 0, "Set toggle hand (0 - left, 1 - right)");

            OnPreferencesSaved();
        }

        public override void OnPreferencesSaved()
        {
            m_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("MCT", "MicToggle");
            m_hand = MelonLoader.MelonPreferences.GetEntryValue<int>("MCT", "MicHand");
            m_hand = Mathf.Clamp(m_hand, 0, 1);
            m_trackedController = null;
            m_buttonOldState = false;
        }

        public override void OnUpdate()
        {
            if (m_enabled)
            {
                if (!m_controllerManager) FindControllerManager();

                if (m_controllerManager)
                {
                    if (!m_trackedController) FindTrackedController();

                    if (m_trackedController)
                    {
                        m_trackedController.Update();

                        if (m_buttonOldState != m_trackedController.field_Public_Boolean_2)
                        {
                            m_buttonOldState = m_trackedController.field_Public_Boolean_2;
                            if (m_buttonOldState)
                            {
                                long l_tick = System.DateTime.Now.Ticks;
                                if ((l_tick - m_lastToggleTick) < m_toggleDelay)
                                {
                                    DefaultTalkController.Method_Public_Static_Void_0();
                                    m_lastToggleTick = l_tick - (m_toggleDelay * 2L);
                                }
                                else m_lastToggleTick = l_tick;
                            }
                        }
                    }
                }
            }
        }
    }
}
