using System;
using System.Collections.Generic;


namespace ZoneRenderer.Console
{
    [Obsolete("The console Zone renderer is not intended for final use, only debugging")]
    public class ConsoleZone : IZoneRenderer
    {
        private RenderedZone activeZone;

        public override void ActivateSector(RenderedZone Zone)
        {
            if(this.activeZone != Zone)
            {
                this.activeZone = Zone;
                if (Zone != null)
                {
                    System.Console.WriteLine($"Active Zone Changed x={Zone.Target.Left}, y={Zone.Target.Top}, width={Zone.Target.Right - Zone.Target.Left}, height={Zone.Target.Bottom - Zone.Target.Top}");
                }
                else
                {
                    System.Console.WriteLine("Active Zone is null!");
                }    
            }
        }

        public override void HideZone()
        {
            System.Console.WriteLine("Zone hidden!");
        }

        public override void RenderZone()
        {
            System.Console.WriteLine("Zone Shown!");
        }

        public override void UpdateZones(Layout Zones)
        {
            System.Console.WriteLine("Zones Updated!");
        }
    }
}
