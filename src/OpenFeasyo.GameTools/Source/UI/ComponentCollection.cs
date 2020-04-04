using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenFeasyo.GameTools.UI
{
    public class ComponentCollection : Component
    {
        #region Properties

        private List<Component> _components = new List<Component>();
        public List<Component> Components { get { return _components; } }

        #endregion


        public override void OnCursorDown(Vector2 pos)
        {
            foreach (Component c in _components)
            {
                if (c.IsIn(pos) && !c.Hidden)
                {
                    c.OnCursorDown(pos);
                }
            }
        }

        public override void OnCursorUp(Vector2 pos)
        {

            foreach (Component c in _components)
            {
                if (c.IsIn(pos) && !c.Hidden)
                {
                    c.OnCursorUp(pos);
                }
            }
        }

        public override bool OnCursorClick(Vector2 pos)
        {
            foreach (Component c in _components)
            {
                if (c.IsIn(pos) && !c.Hidden)
                {
                    if (c.OnCursorClick(pos))
                        return true;
                }
            }
            return false;
        }

     
        public override void OnCursorMove(Vector2 oldPos, Vector2 newPos) 
        {
            foreach (Component c in _components)
            {
                if (c.Hidden)
                {
                    continue;
                }

                if (c.IsIn(newPos) )
                {
                    c.OnCursorMove(oldPos, newPos);
                    if (!c.IsIn(oldPos))
                    {
                        c.OnCursorEnter(newPos);
                    }
                }
                else {
                    if (!c.IsIn(newPos)) {
                        c.OnCursorLeave(oldPos);
                    }
                }
            }
        }

        public override void OnMoveGesture(Vector2 moveVec)
        {
            foreach (Component c in _components)
            {
                if (!c.Hidden)
                { 
                    c.OnMoveGesture(moveVec);
                }
        
            }
        }

        public override void OnResizeGesture(float oldSize, float newSize)
        {
            foreach (Component c in _components)
            {
                if (!c.Hidden)
                {
                    c.OnResizeGesture(oldSize, newSize);

                }
            }
        }



        public override void Update(GameTime gameTime)
        {
            foreach (Component c in _components)
            {
                c.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            foreach (Component c in _components)
            {
                if (!c.Hidden)
                {
                    c.Draw(gameTime, spritebatch);
                }
            }
        }
    }
}
