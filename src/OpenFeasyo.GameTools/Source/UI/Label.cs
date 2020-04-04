using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenFeasyo.GameTools.UI
{
	public class Label : Component
	{
		private SpriteFont _font;
		private string _text;
		private Color _color;

		public string Text
		{
			get { return _text; }
			set {
				_text = value;
				this.Size = _font.MeasureString(_text);
			}
		}

		public Label(string text, SpriteFont font,Color color) : base()
		{
			_text = text;
			_font = font;
			_color = color;
			this.Size = _font.MeasureString(text);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
		{
			spritebatch.DrawString(_font,_text,Position,_color);
		}


	}
}