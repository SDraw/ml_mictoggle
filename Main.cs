using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml_mictoggle
{
    public class MCT : MelonLoader.MelonMod
    {
        SteamVR_ControllerManager m_controllerManager = null;
        SteamVR_TrackedController m_trackedController = null;

        private const long m_toggleDelay = 5000000L;

        private bool m_toggleEnabled = true;
        private bool m_buttonOldState = false;
        private long m_lastToggleTick = 0L;
        private bool m_micState = false;

        private void FindControllerManager()
        {
            VRCInputProcessor l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Vive];
            if (l_input)
            {
                VRCInputProcessorVive l_castInput = l_input.Cast<VRCInputProcessorVive>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }

            l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.ViveAdvanced];
            if (l_input)
            {
                VRCInputProcessorViveAdvanced l_castInput = l_input.Cast<VRCInputProcessorViveAdvanced>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }

            l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Oculus];
            if (l_input)
            {
                VRCInputProcessorTouch l_castInput = l_input.Cast<VRCInputProcessorTouch>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }

            l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Index];
            if (l_input)
            {
                VRCInputProcessorIndex l_castInput = l_input.Cast<VRCInputProcessorIndex>();
                if (l_castInput)
                {
                    m_controllerManager = l_castInput.field_Private_SteamVR_ControllerManager_0;
                    return;
                }
            }
        }

        private void FindTrackedController()
        {
            m_trackedController = m_controllerManager.left.GetComponent<SteamVR_TrackedController>();
            if (!m_trackedController) m_trackedController = m_controllerManager.left.AddComponent<SteamVR_TrackedController>();
        }

        public override void OnApplicationStart()
        {
            MelonLoader.MelonPrefs.RegisterCategory("MCT", "Microphone fast VR toggle");
            MelonLoader.MelonPrefs.RegisterBool("MCT", "MicToggle", true, "Mic fast toggle");
        }

        public override void OnModSettingsApplied()
        {
            m_toggleEnabled = MelonLoader.MelonPrefs.GetBool("MCT", "MicToggle");
        }

        public override void OnUpdate()
        {
            if (m_toggleEnabled)
            {
                if (!m_controllerManager) FindControllerManager();

                if (m_controllerManager)
                {
                    if (!m_trackedController) FindTrackedController();

                    if (m_trackedController)
                    {
                        m_trackedController.Update();

                        if (m_buttonOldState != m_trackedController.menuPressed)
                        {
                            m_buttonOldState = m_trackedController.menuPressed;
                            if (m_buttonOldState)
                            {
                                long l_tick = System.DateTime.Now.Ticks;
                                if ((l_tick - m_lastToggleTick) < m_toggleDelay)
                                {
                                    m_micState = !m_micState;
                                    if (m_micState) DefaultTalkController.Method_Public_Static_Void_PDM_6();
                                    else DefaultTalkController.Method_Public_Static_Void_PDM_5();

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
