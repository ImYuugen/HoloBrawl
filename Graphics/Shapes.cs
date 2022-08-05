using System;
using HoloBrawl.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Graphics;

public sealed class Shapes : IDisposable
{
    public const float MinThickness = 2f;
    public const float MaxThickness = 100f;

    private Game _game;
    private BasicEffect _effect;

    private VertexPositionColor[] _vertices;
    private int[] _indices;

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
        
        const int MaxVertices = 1024;
        const int MaxIndices = MaxVertices * 3;
        
        _vertices = new VertexPositionColor[MaxVertices];
        _indices = new int[MaxIndices];
        
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
    
    public void Begin()
    {
        if (_isStarted)
            throw new InvalidOperationException("Shapes.Begin() was called twice without calling Shapes.End()");
        
        var vp = _game.GraphicsDevice.Viewport;
        _effect.Projection = 
            Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
        
        _isStarted = true;
    }

    public void End()
    {
        Flush();
        _isStarted = false;
    }

    public void Flush()
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
    
    public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
    {
        EnsureStarted();
        
        const int shapeVertexCount = 4;
        const int shapeIndexCount = 6;
        EnsureSpace(shapeVertexCount, shapeIndexCount);

        thickness = Utils.Clamp(thickness, MinThickness, MaxThickness);
        var halfThickness = thickness / 2f;
        
        var e1 = b - a;
        e1.Normalize();
        e1 *= halfThickness;
        var e2 = -e1;

        var n1 = new Vector2(-e1.Y, e1.X);
        var n2 = -n1;
        
        var q1 = a + e2 + n1;
        var q2 = b + e1 + n1;
        var q3 = b + e1 + n2;
        var q4 = a + e2 + n2;
        
        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 1;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount;
        _indices[_indexCount++] = _vertexCount + 2;
        _indices[_indexCount++] = _vertexCount + 3;
        
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q1, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q2, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q3, 0f), color);
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(q4, 0f), color);

        _shapesCount++;
    }
    
    
    public void EnsureStarted()
    {
        if (!_isStarted)
            throw new InvalidOperationException("[ERROR] Batching is not started, call Shapes.Begin() first");
    }

    public void EnsureSpace(int vertexCount, int indexCount)
    {
        if (vertexCount > _vertices.Length)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "[ERROR] Vertex count is too big, passed " + vertexCount + ", max is " + _vertices.Length);
        if (indexCount > _indices.Length)
            throw new ArgumentOutOfRangeException(nameof(indexCount), "[ERROR] Index count is too big, passed " + indexCount + ", max is " + _indices.Length);
        
        if (_vertexCount + vertexCount > _vertices.Length || _indexCount + indexCount > _indices.Length)
            Flush();
    }
}