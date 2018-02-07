using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Extensions.DebugView;
using VelcroPhysics.Shared;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;


namespace ArcanistsRevamped
{
    class GameDebugView : DebugViewBase
    {
        private World world;

        public GameDebugView(World world)
            :base(world)
        {
            this.world = world;
        }

        public override void DrawCircle(Vector2 center, float radius, float red, float blue, float green)
        {
            
        }

        public override void DrawPolygon(Vector2[] vertices, int count, float red, float blue, float green, bool closed = true)
        {
            throw new NotImplementedException();
        }

        public override void DrawSegment(Vector2 start, Vector2 end, float red, float blue, float green)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float blue, float green)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidPolygon(Vector2[] vertices, int count, float red, float blue, float green)
        {
            throw new NotImplementedException();
        }

        public override void DrawTransform(ref Transform transform)
        {
            throw new NotImplementedException();
        }
    }
}
