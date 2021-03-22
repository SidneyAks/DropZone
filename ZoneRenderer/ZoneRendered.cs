using System;

namespace ZoneRenderer
{
    [Serializable]
    public class RenderedZone : ZoneBase<int>
    {
        public Zone Zone { get; set; }
        public new Bounds<int> Trigger { get => base.Trigger ?? base.Target; set => base.Trigger = value; }

        public int TargetWidth => targetWidth ?? (targetWidth = Target.Right - Target.Left).Value;
        private int? targetWidth;

        public int TriggerWidth => triggerWidth ?? (triggerWidth = Trigger.Right - Trigger.Left).Value;
        private int? triggerWidth;

        public int TargetHeight => targetHeight ?? (targetHeight = Target.Bottom - Target.Top).Value;
        private int? targetHeight;

        public int TriggerHeight => triggerHeight ?? (triggerHeight = Trigger.Bottom - Trigger.Top).Value;
        private int? triggerHeight;
    }
}
