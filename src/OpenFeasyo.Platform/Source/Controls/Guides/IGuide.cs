using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.Platform.Controls.Reports;

namespace OpenFeasyo.Platform.Controls.Guides
{
    public interface IGuide
    {
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
        void OnReport(IReport report);

        bool Hidden { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
        Color Background { get; set; }
        bool ActiveZoom { get; set; }

        bool VisualFeedback { get; set; }
    }
}
