using System.Runtime.InteropServices;

namespace AvatarViewer.Native.Win
{
    public enum _D3DKMT_SCHEDULINGPRIORITYCLASS
    {
        D3DKMT_SCHEDULINGPRIORITYCLASS_IDLE,
        D3DKMT_SCHEDULINGPRIORITYCLASS_BELOW_NORMAL,
        D3DKMT_SCHEDULINGPRIORITYCLASS_NORMAL,
        D3DKMT_SCHEDULINGPRIORITYCLASS_ABOVE_NORMAL,
        D3DKMT_SCHEDULINGPRIORITYCLASS_HIGH,
        D3DKMT_SCHEDULINGPRIORITYCLASS_REALTIME
    }

    public static class GPUPriority
    {

        [DllImport("AvatarViewer.Native.Win", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChangePriority(_D3DKMT_SCHEDULINGPRIORITYCLASS processPriority, int gpuThreadPriority);

    }
}
