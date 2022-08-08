using System;
using HoloBrawl.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Graphics;

public sealed class Shapes : IDisposable
{
    private const float MinThickness = 2f;
    private const float MaxThickness = 100f;

    private readonly Game _game;
    private readonly BasicEffect _effect;

    private readonly VertexPositionColor[] _vertices;
    private readonly int[] _indices;

    private int _shapesCount;
    private int _vertexCount;
    private int _indexCount;

    private bool _isStarted;
    private bool _isDisposed;
    
    public Shapes(Game game)
    {
        _isDisposed = false;
        _game = game ?? throw new ArgumentNullException(nameof(game), "Game was null in Shapes constructor");
        _effect = new BasicEffect(_game.GraphicsDevice)
        {
            TextureEnabled = false,
            FogEnabled = false,
            LightingEnabled = false,
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.Identity
        };
        
        const int maxVertices = 1024;
        const int maxIndices = maxVertices * 3;
        
        _vertices = new VertexPositionColor[maxVertices];
        _indices = new int[maxIndices];
        
        _shapesCount = 0;
        _vertexCount = 0;
        _indexCount = 0;
        
        _isStarted = false;
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        
        _effect?.Dispose();
        _isDisposed = true;
    }
    
    public void Begin(Camera camera)
    {
        if (_isStarted)
            throw new InvalidOperationException("Shapes.Begin() was called twice without calling Shapes.End()");
     
        
        if (camera is null)
        {
            var vp = _game.GraphicsDevice.Viewport;
            _effect.Projection = 
                Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
            _effect.View = Matrix.Identity;
        }
        else
        {
            camera.UpdateMatrices();
            _effect.View = camera.View;
            _effect.Projection = camera.Projection;
        }
        
        _isStarted = true;
    }

    public void End()
    {
        Flush();
        _isStarted = false;
    }

    private void Flush()
    {
        if (_shapesCount == 0)
            return;
        
        EnsureStarted();

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _game.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                _vertices,
                0,
                _vertexCount,
                _indices,
                0,
                _indexCount / 3); // 3 indices per triangle
        }

        _shapesCount = 0;
        _vertexCount = 0;
        _indexCount = 0;
    }

    /// <summary>
    /// Draws a Rectangle.
    /// </summary>
    /// <param name="x">The x coordinate of the bottom left corner of the rectangle</param>
    /// <param name="y">The y coordinate of the bottom left corner of the rectangle</param>
    /// <param name="width">The width of the rectangle in pixels</param>
    /// <param name="height">The height pf the rectangle in pixels</param>
    /// <param name="color">The color of the rectangle</param>
    public void DrawRectangle(float x, float y, float width, float height, Color color)
    {
        EnsureStarted();
        
        const int shapeVertexCount = 4;
        const int shapeIndexCount = 6;
        
        EnsureSpace(shapeVertexCount, shapeIndexCount); 
        
        float left = x, right = x + width, top = y, bottom = y + height;
        Vector2 a = new(left, top), b = new(right, top), c = new(right, bottom), d = new(left, bottom);
        
        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 1;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount + 3;
        
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

        _shapesCount++;
    }
    
    /// <summary>
    /// Wrapper for DrawRectangle that takes a Rectangle as input.
    /// </summary>
    /// <param name="rectangle">The desired rectangle</param>
    /// <param name="color">The desired color</param>
    public void DrawRectangle(Rectangle rectangle, Color color)
    {
        DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
    }

    private void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color)
    {
        EnsureStarted();
        
        const int shapeVertexCount = 4;
        const int shapeIndexCount = 6;
        EnsureSpace(shapeVertexCount, shapeIndexCount);

        thickness = Utils.Clamp(thickness, MinThickness, MaxThickness);
        var halfThickness = thickness / 2f;

        float e1X = bx - ax, e1Y = by - ay;
        Utils.Normalize(ref e1X, ref e1Y);
        e1X *= halfThickness;
        e1Y *= halfThickness;
        
        float e2X = -e1X, e2Y = -e1Y;

        float n1X = -e1Y, n1Y = e1X;
        float n2X = -n1X, n2Y = -n1Y;

        float q1X = ax + e2X + n1X, q1Y = ay + e2Y + n1Y;
        float q2X = bx + e1X + n1X, q2Y = by + e1Y + n1Y;
        float q3X = bx + e1X + n2X, q3Y = by + e1Y + n2Y;
        float q4X = ax + e2X + n2X, q4Y = ay + e2Y + n2Y;

        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 1;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount + 3;
        
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q1X, q1Y, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q2X, q2Y, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q3X, q3Y, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q4X, q4Y, 0f), color);

        _shapesCount++;
    }
    
    /// <summary>
    /// Wrapper for DrawLine
    /// </summary>
    /// <param name="a">Point A</param>
    /// <param name="b">Point B</param>
    /// <param name="thickness">The thickness of the line</param>
    /// <param name="color">The color of the line</param>
    public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
    {
        DrawLine(a.X, a.Y, b.X, b.Y, thickness, color);
    }


    private void EnsureStarted()
    {
        if (!_isStarted)
            throw new InvalidOperationException("[ERROR] Batching is not started, call Shapes.Begin() first");
    }

    private void EnsureSpace(int vertexCount, int indexCount)
    {
        if (vertexCount > _vertices.Length)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "[ERROR] Vertex count is too big, passed " + vertexCount + ", max is " + _vertices.Length);
        if (indexCount > _indices.Length)
            throw new ArgumentOutOfRangeException(nameof(indexCount), "[ERROR] Index count is too big, passed " + indexCount + ", max is " + _indices.Length);
        
        if (_vertexCount + vertexCount > _vertices.Length || _indexCount + indexCount > _indices.Length)
            Flush();
    }
}