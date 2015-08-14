using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Puzzle3D {

	public class Chip {

		public enum RevolveDirection {
			Up = 0,
			Down = 1,
			Left = 2,
			Right = 3
		};


		int xPosition;
		int yPosition;
		bool horizontalRotationCorrect = true;
		bool verticalRotationCorrect = true;

		float colorOverride = 1f;
		float glowScale;
		Vector2 texCoordScale;
		Vector2 texCoordTranslationFront;
		Vector2 texCoordTranslationBack;

		bool isRevolving;
		Matrix orientationMatrix = Matrix.Identity;
		RevolveDirection revolutionDirection;
		float revolutionDuration;
		float currentRevolutionTime;
		float currentRevolutionY;
		float targetRevolutionY;
		float currentRevolutionX;
		float targetRevolutionX;
		public const float FlipDuration = 0.65f;



		public Matrix OrientationMatrix {
			get { return orientationMatrix; }
		}

		public bool HorizontalRotationCorrect {
			get { return horizontalRotationCorrect; }
		}

		public bool VerticalRotationCorrect {
			get { return verticalRotationCorrect; }
		}

		public int XPosition {
			get { return xPosition; }
			set { xPosition = value; }
		}

		public int YPosition {
			get { return yPosition; }
			set { yPosition = value; }
		}

		public float ColorOverride {
			get { return colorOverride; }
			set { colorOverride = value; }
		}

		public float GlowScale {
			get { return glowScale; }
			set { glowScale = value; }
		}

		public Vector2 TexCoordScale {
			get { return texCoordScale; }
			set { texCoordScale = value; }
		}

		public Vector2 TexCoordTranslationFront {
			get { return texCoordTranslationFront; }
			set { texCoordTranslationFront = value; }
		}

		public Vector2 TexCoordTranslationBack {
			get { return texCoordTranslationBack; }
			set { texCoordTranslationBack = value; }
		}



		public static RevolveDirection GetRandomDirection() {
			return (RevolveDirection)RandomHelper.Random.Next( 4 );
		}


		public void Flip( RevolveDirection direction ) {
			if( isRevolving )
				return;

			float amount = MathHelper.Pi;

			Audio.Play( "Flip Chip" );

			switch( direction ) {
				case RevolveDirection.Up:
					if( !horizontalRotationCorrect )
						amount = -amount;
					targetRevolutionX = currentRevolutionX - amount;
					break;

				case RevolveDirection.Down:
					if( !horizontalRotationCorrect )
						amount = -amount;
					targetRevolutionX = currentRevolutionX + amount;
					break;

				case RevolveDirection.Left:
					targetRevolutionY = currentRevolutionY - amount;
					break;

				case RevolveDirection.Right:
					targetRevolutionY = currentRevolutionY + amount;
					break;
			}

			this.revolutionDirection = direction;
			this.revolutionDuration = FlipDuration;
			this.currentRevolutionTime = 0.0f;
			this.isRevolving = true;
		}



		public void Update( GameTime gameTime ) {
			if( !isRevolving )
				return;

			currentRevolutionTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if( currentRevolutionTime >= revolutionDuration ) {
				currentRevolutionTime = 0.0f;
				isRevolving = false;

				switch( revolutionDirection ) {
					case RevolveDirection.Left:
					case RevolveDirection.Right:
						currentRevolutionY = targetRevolutionY;
						horizontalRotationCorrect = !horizontalRotationCorrect;
						break;

					case RevolveDirection.Up:
					case RevolveDirection.Down:
						currentRevolutionX = targetRevolutionX;
						verticalRotationCorrect = !verticalRotationCorrect;
						break;
				}

				orientationMatrix = Matrix.CreateRotationX( currentRevolutionX ) * Matrix.CreateRotationY( currentRevolutionY );
			} else {
				const float stretchFactor = 0.2f;
				float revolutionFraction = currentRevolutionTime / revolutionDuration;
				revolutionFraction *= ( 1.0f + ( 2.0f * stretchFactor ) );
				revolutionFraction -= stretchFactor;
				revolutionFraction *= MathHelper.Pi;

				float rotationValue = ( revolutionFraction - MathHelper.PiOver2 );
				rotationValue = (float)Math.Sin( rotationValue );
				float overflowFactor = (float)Math.Sin( MathHelper.PiOver2 + ( MathHelper.Pi * stretchFactor ) );
				rotationValue = ( ( rotationValue * ( 1.0f / overflowFactor ) ) + 1.0f ) / 2.0f;

				switch( revolutionDirection ) {
					case RevolveDirection.Left:
					case RevolveDirection.Right:
						float tempRevolutionY = MathHelper.Lerp( currentRevolutionY, targetRevolutionY, rotationValue );
						orientationMatrix = Matrix.CreateRotationX( currentRevolutionX ) * Matrix.CreateRotationY( tempRevolutionY );
						break;

					case RevolveDirection.Up:
					case RevolveDirection.Down:
						float tempRevolutionX = MathHelper.Lerp( currentRevolutionX, targetRevolutionX, rotationValue );
						orientationMatrix = Matrix.CreateRotationX( tempRevolutionX ) * Matrix.CreateRotationY( currentRevolutionY );
						break;
				}
			}
		}

		public void Draw( Board board, Model chipModel, Matrix baseTransform, Matrix[] chipTransforms ) {
			Matrix world = OrientationMatrix * baseTransform;
			LightingEffect lightingEffect = board.LightingEffect;

			lightingEffect.GlowScale.SetValue( GlowScale );
			lightingEffect.ColorOverride.SetValue( ColorOverride );
			lightingEffect.TexCoordScale.SetValue( TexCoordScale );

			foreach( ModelMesh mesh in chipModel.Meshes ) {
				Matrix modelWorld = chipTransforms[ mesh.ParentBone.Index ] * world;
				Matrix modelWorldView = modelWorld * board.Camera.ViewMatrix;
				Matrix modelWorldViewProjection = modelWorldView * board.Camera.ProjectionMatrix;

				lightingEffect.World.SetValue( modelWorld );
				lightingEffect.WorldView.SetValue( modelWorldView );
				lightingEffect.WorldViewProjection.SetValue( modelWorldViewProjection );

				foreach( ModelMeshPart meshPart in mesh.MeshParts ) {
					bool textureSet = false;
					string materialIdentifier = meshPart.Tag as string;
					if( materialIdentifier != null ) {
						switch( materialIdentifier ) {
							case "Front":
								lightingEffect.TexCoordTranslation.SetValue( this.TexCoordTranslationFront );
								textureSet = SetTexture( board, 0 );
								break;
							case "Back":
								lightingEffect.TexCoordTranslation.SetValue( this.TexCoordTranslationBack );
								if( board.TwoSided )
									textureSet = SetTexture( board, 1 );
								else
									textureSet = SetTexture( board, 0 );
								break;
							default:
								lightingEffect.DiffuseTexture.SetValue( (Texture2D)null );
								break;
						}
					}

					if( textureSet ) {
						lightingEffect.Effect.CurrentTechnique = lightingEffect.SingleTextureTechnique;
					} else {
						lightingEffect.Effect.CurrentTechnique = lightingEffect.NoTextureTechnique;
					}

					lightingEffect.Effect.Begin();
					DrawHelper.DrawMeshPart( mesh, meshPart, lightingEffect.Effect );
					lightingEffect.Effect.End();
				}
			}
		}

		static bool SetTexture( Board board, int textureIndex ) {
			LightingEffect lightingEffect = board.LightingEffect;
			Texture2D texture = null;

			if( board.CurrentPictureSet != null )
				texture = board.CurrentPictureSet.GetTexture( textureIndex );

			if( ( texture == null ) && ( board.NextPictureSet != null ) )
				texture = board.NextPictureSet.GetTexture( textureIndex );

			lightingEffect.DiffuseTexture.SetValue( texture );

			return ( texture != null );
		}

	}

}
