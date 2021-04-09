using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoneRenderer
{
    public class RenderedLayout
    {
        public readonly Layout Base;
        public readonly List<RenderedZone> Zones;


        public RenderedLayout(Layout layoutBase, ScreenInfo.DisplayInfoCollection displayinfo = null, Layout Parent = null)
        {
            Base = layoutBase;

            var parentZones = Parent?.Render(displayinfo).Zones;

            Zones = parentZones?.Union(RenderZones(layoutBase, displayinfo)).ToList() ?? RenderZones(layoutBase, displayinfo);
        }

        private List<RenderedZone> RenderZones (Layout layoutBase, ScreenInfo.DisplayInfoCollection displayinfo = null)
        {
            displayinfo = displayinfo ?? ScreenInfo.GetDisplays();
            return layoutBase.List.SelectMany<IRenderableZoneBase<IRenderableBound>, RenderedZone>(zone =>
                //If layout is duplicated, create layout once per each monitor using said monitors bounds
                (zone.Layout == LayoutKind.Duplicated ? displayinfo.Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )).OfType<RenderedZone>() :
                //If layout is spanning, create layout once using full work area bounds
                zone.Layout == LayoutKind.Spanning ? new RenderedZone[] {zone.Render(
                        x: displayinfo.Min(y => y.WorkArea.Left),
                        y: displayinfo.Min(y => y.WorkArea.Top),
                        LayoutWidth: displayinfo.MaxWidth - displayinfo.Min(y => y.WorkArea.Left),
                        LayoutHeight: displayinfo.MaxHeight - displayinfo.Min(y => y.WorkArea.Top)
                    )}.OfType<RenderedZone>() :
                //If layout is selected screans, create layout once per selected monitor, using said monitors bounds
                zone.Layout == LayoutKind.SelectedScreens ? zone.ScreenIndexes
                                                                    .Where(x => x < displayinfo.Count())
                                                                    .Select(x => displayinfo[x])
                                                                    .Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )).OfType<RenderedZone>() :
                throw new Exception("Unknown layout kind (how did you even do that with an enum?"))
            ).ToList();

        }

        public RenderedZone GetActiveZoneFromPoint(int x, int y)
        {
            return Zones.FirstOrDefault(rect =>
                rect.Trigger.Left < x && x < rect.Trigger.Right &&
                rect.Trigger.Top < y && y < rect.Trigger.Bottom);
        }
    }
}
