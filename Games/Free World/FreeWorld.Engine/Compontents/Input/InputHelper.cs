﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FreeWorld.Engine.Compontents.Input {

	public class InputHelper : DrawableGameComponent {

		private SpriteBatch mSpriteBatch;
		private SpriteFont mSpriteFont;

		private KeyboardState mCurrentKeyState;
		private KeyboardState mPreviousKeyState;

		private MouseState mCurrentMouseState;
		private MouseState mPreviousMouseState;

		private Vector2 mMousePosition;

		private static ActionMap[] mActions;

		public Vector2 MousePosition {
			get { return mMousePosition; }
		}

		public float ScrollWheelDelta {
			get { return MathHelper.Clamp( ( mCurrentMouseState.ScrollWheelValue - mPreviousMouseState.ScrollWheelValue ), -1, 1 ); }
		}

		public static ActionMap[] ActionMap {
			get { return mActions; }
		}

		public Keys[] PressedKeys {
			get { return mCurrentKeyState.GetPressedKeys(); }
		}


		public InputHelper( Game game )
			: base( game ) {

			mPreviousKeyState = mCurrentKeyState = Keyboard.GetState();

			mPreviousMouseState = mCurrentMouseState = Mouse.GetState();
			mMousePosition = new Vector2( mCurrentMouseState.X, mCurrentMouseState.Y );

			ResetActionMap();
		}

		private static void ResetActionMap() {
			EActions[] actions = (EActions[])Enum.GetValues( typeof( EActions ) );
			mActions = new ActionMap[ (int)EActions.ActionCount ];

			mActions[ (int)EActions.ExitGame ] = new ActionMap( "Testclient schließen", Keys.Escape );
			mActions[ (int)EActions.MoveUp ] = new ActionMap( "Charakter bewegen - Hoch", new Keys[] { Keys.Up, Keys.W } );
			mActions[ (int)EActions.MoveLeft ] = new ActionMap( "Charakter bewegen - Links", new Keys[] { Keys.Left, Keys.A } );
			mActions[ (int)EActions.MoveDown ] = new ActionMap( "Charakter bewegen - Runter", new Keys[] { Keys.Down, Keys.S } );
			mActions[ (int)EActions.MoveRight ] = new ActionMap( "Charakter bewegen - Rechts", new Keys[] { Keys.Right, Keys.D } );
		}

		protected override void LoadContent() {
			mSpriteBatch = new SpriteBatch( Constants.GraphicsDevice );
			mSpriteFont = EngineCore.ContentLoader.Load<SpriteFont>( @"Fonts\FPS" );

			base.LoadContent();
		}

		public override void Update( GameTime gameTime ) {
			mPreviousMouseState = mCurrentMouseState;
			mCurrentMouseState = Mouse.GetState();

			mMousePosition.X = mCurrentMouseState.X;
			mMousePosition.Y = mCurrentMouseState.Y;

			mPreviousKeyState = mCurrentKeyState;
			mCurrentKeyState = Keyboard.GetState();

			base.Update( gameTime );
		}

		public override void Draw( GameTime gameTime ) {
			base.Draw( gameTime );
			if( Constants.GraphicsDevice == null ) // Client exit
				return;

			string state = String.Format( "X: {0}\nY: {1}", mMousePosition.X, mMousePosition.Y );
			Vector2 vector_state = mSpriteFont.MeasureString( state );
			Vector2 pos = new Vector2( Constants.GraphicsDevice.Viewport.Width - 40f - vector_state.X, Constants.GraphicsDevice.Viewport.Height - 40f - vector_state.Y );

			mSpriteBatch.Begin();
			mSpriteBatch.DrawString( mSpriteFont, state, pos, Color.White );
			mSpriteBatch.End();

		}



		public bool WasMouseButtonPressed( EMouseButtons mb ) {
			return ( IsMouseButtonDown( mb ) && IsMouseButtonState( mb, ButtonState.Released, mPreviousMouseState ) );
		}

		public bool IsMouseButtonReleased( EMouseButtons mb ) {
			return ( IsMouseButtonUp( mb ) && IsMouseButtonState( mb, ButtonState.Pressed, mPreviousMouseState ) );
		}

		public bool IsMouseButtonDown( EMouseButtons mb ) {
			return IsMouseButtonState( mb, ButtonState.Pressed, mCurrentMouseState );
		}

		public bool IsMouseButtonUp( EMouseButtons mb ) {
			return IsMouseButtonState( mb, ButtonState.Released, mCurrentMouseState );
		}

		private bool IsMouseButtonState( EMouseButtons mb, ButtonState state, MouseState mouseState ) {
			switch( mb ) {
				case EMouseButtons.LeftButton:
					return ( mouseState.LeftButton == state );
				case EMouseButtons.MiddleButton:
					return ( mouseState.MiddleButton == state );
				case EMouseButtons.RightButton:
					return ( mouseState.RightButton == state );
				case EMouseButtons.XButton1:
					return ( mouseState.XButton1 == state );
				case EMouseButtons.XButton2:
					return ( mouseState.XButton2 == state );
			}
			return false;
		}


		public bool WasKeyPressed( Keys key ) {
			return ( mCurrentKeyState.IsKeyDown( key ) && mPreviousKeyState.IsKeyUp( key ) );
		}

		public bool IsKeyReleased( Keys key ) {
			return ( mCurrentKeyState.IsKeyUp( key ) && mPreviousKeyState.IsKeyDown( key ) );
		}

		public bool IsKeyDown( Keys key ) {
			return mCurrentKeyState.IsKeyDown( key );
		}

		public bool IsKeyUp( Keys key ) {
			return mCurrentKeyState.IsKeyUp( key );
		}



		public bool IsActionPressed( EActions action ) {
			return IsActionMapPressed( mActions[ (int)action ] );
		}

		private bool IsActionMapPressed( ActionMap actionMap ) {
			for( int i = 0; i < actionMap.Keys.Count; i++ )
				if( IsKeyDown( actionMap.Keys[ i ] ) == true )
					return true;
			return false;
		}

		public static string GetActionName( EActions action ) {
			int iAction = (int)action;
			if( iAction < 0 || iAction >= mActions.Length )
				return string.Empty;

			return mActions[ iAction ].Name;
		}

	}

}
