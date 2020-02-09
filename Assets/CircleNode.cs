using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;

//Reference: https://docs.unity3d.com/Packages/com.unity.vectorgraphics@2.0/api/Unity.VectorGraphics.html

//Of great help was https://github.com/Unity-Technologies/vector-graphics-samples/blob/master/Assets/RuntimeDemo/SVGRuntimeLoad.cs
//Which contained a working example of the library populating a sprite.
//I was able to use the MonoDevelop debugger to find out the scene object being generated from the svg string in that example,
//and this helped me find out about the negative coordinate bug and produce a working scene.
public class CircleNode : MonoBehaviour
{
	public float diameter = 1;
	public float thickness = 0.1f;
	private SpriteRenderer myRenderer;
	private void Start()
	{
		myRenderer = GetComponent<SpriteRenderer>();

		var baseShape = new Shape
		{
			Fill = new SolidFill
			{
				Color = Color.black,
				Mode = FillMode.NonZero,
				Opacity = 1f
			},
			PathProps = new PathProperties
			{
				Stroke = new Stroke
				{
					Color = Color.red,
					HalfThickness = thickness/2,
				},
			}
		};

		//NOTE: Not sure if this a bug, but for some reason the contours CANNOT have negative coordinates when rendering to sprite, even if the sprite rect is set negative.
		//So, make sure your circle stays in positive coordinates.
		VectorUtils.MakeCircleShape(baseShape, Vector2.one * (diameter/2 + thickness/2), diameter/2);

		var scene = new Scene
		{
			Root = new SceneNode
			{
				Shapes = new List<Shape>
				{
					baseShape,
				},
			}
		};

		var geoms = VectorUtils.TessellateScene(scene, new VectorUtils.TessellationOptions()
		{
			StepDistance = 10,
			MaxCordDeviation = 1,
			MaxTanAngleDeviation = 0.1f,
			SamplingStepSize = 0.01f
		});

		myRenderer.sprite = VectorUtils.BuildSprite(geoms, new Rect(0, 0, diameter + thickness, diameter + thickness), 1, VectorUtils.Alignment.Center, Vector2.zero, 0, false);
	}
}

/* Example: Make a custom bezier shape. (Use this instead of MakeCircleShape)
 * This bezier is just a square, but the P0, P1, P2 handles work just like bezier handles in any art program.
 * Again, the negative coordinates rule applies, though possibly handle coords can be negative so long as the curve itself doesn't go negative. Not sure.
 * /*
baseShape.Contours = new Unity.VectorGraphics.BezierContour[]
{
	new Unity.VectorGraphics.BezierContour
	{
		Closed = true,
		Segments = new Unity.VectorGraphics.BezierPathSegment[]
		{
			new Unity.VectorGraphics.BezierPathSegment
			{
				P0 = new Vector2(-10, 0),
				P1 = new Vector2(-10, 0),
				P2 = new Vector2(-10, 0)
			},
			new Unity.VectorGraphics.BezierPathSegment
			{
				P0 = new Vector2(0, 10),
				P1 = new Vector2(0, 10),
				P2 = new Vector2(0, 10)
			},
						
			new Unity.VectorGraphics.BezierPathSegment
			{
				P0 = new Vector2(10, 10),
				P1 = new Vector2(10, 10),
				P2 = new Vector2(10, 10)
			},
			new Unity.VectorGraphics.BezierPathSegment
			{
				P0 = new Vector2(10, 0),
				P1 = new Vector2(10, 0),
				P2 = new Vector2(10, 0)
			}
		}
	}
},
*/
