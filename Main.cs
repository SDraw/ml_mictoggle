using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml_mictoggle
{
    public class MCT : MelonLoader.MelonMod
    {
        private const long m_toggleDelay = 5000000L;
        private bool m_toggleEnabled = true;
        private bool m_toggleState = false;
        private long m_lastToggleTick = 0L;
        private bool m_buttonOldState = false;

        public override void OnApplicationStart()
        {
            MelonLoader.ModPrefs.RegisterCategory("MCT", "Microphone fast VR toggle");
            MelonLoader.ModPrefs.RegisterPrefBool("MCT", "MicToggle", true, "Mic fast toggle");
        }

        public override void OnModSettingsApplied()
        {
            m_toggleEnabled = MelonLoader.ModPrefs.GetBool("MCT", "MicToggle");
        }

        public override void OnUpdate()
        {
            if (m_toggleEnabled)
            {
                var l_inputProc = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Vive].Cast<VRCInputProcessorVive>();
                if (l_inputProc)
                {
                    var l_controllerManager = l_inputProc.field_Private_SteamVR_ControllerManager_0;
                    if (l_controllerManager)
                    {
                        bool l_buttonNewState = false;

                        // Left hand controller only check
                        if ((l_controllerManager.field_Private_UInt32_0 != Valve.VR.OpenVR.k_unTrackedDeviceIndexInvalid) && (l_controllerManager.left != null))
                        {
                            var l_controller = l_controllerManager.left.GetComponent<SteamVR_TrackedController>();
                            if (l_controller == null) l_controller = l_controllerManager.left.AddComponent<SteamVR_TrackedController>();
                            l_controller.Update();

                            l_buttonNewState = l_controller.menuPressed;
                        }

                        if (m_buttonOldState != l_buttonNewState)
                        {
                            m_buttonOldState = l_buttonNewState;

                            if (!m_buttonOldState)
                            {
                                long l_tick = System.DateTime.Now.Ticks;
                                if ((l_tick - m_lastToggleTick) < m_toggleDelay)
                                {
                                    m_toggleState = !m_toggleState;
                                    if (m_toggleState == true) DefaultTalkController.Method_Public_Static_Void_5();
                                    else DefaultTalkController.Method_Public_Static_Void_4();

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
