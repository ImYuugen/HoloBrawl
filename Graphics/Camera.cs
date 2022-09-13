using System;
using System.Linq;
using Microsoft.Xna.Framework;
using VBrawler.Core;

namespace VBrawler.Graphics
{
    public sealed class Camera
    {
        private const float MinZ = 1f;
        private const float MaxZ = 2048f;
        private const int MinZoom = 1;
        private const int MaxZoom = 8;
        

        private readonly float _fov;
        private readonly float _aspectRatio;

        public Vector2 Position { get; private set; }
        public float BaseZ { get; }
        public float Z { get; private set; }
        public int Zoom { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen), "[ERROR] Screen is null in Camera constructor.");
            
            _aspectRatio = (float)screen.Width / screen.Height;
            _fov = MathHelper.PiOver2;

            Zoom = 1;
            
            Position = Vector2.Zero;
            BaseZ = GetZFromHeight(screen.Height);
            Z = BaseZ;

            UpdateMatrices();
        }

        public void UpdateMatrices()
        {
            View = Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Z), new Vector3(Position.X, Position.Y, 0), Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(_fov, _aspectRatio, MinZ, MaxZ);
        }
        
        private float GetZFromHeight(float height)
        {
            return .5f * height / (float)Math.Tan(_fov * .5f); // z = (height/2) / tan(fov/2)
        }
        
        private float GetHeightFromZ()
        {
            return 2 * Z * (float)Math.Tan(_fov * .5f); // height = z * tan(fov/2)
        }
        
        public void MoveZ(float amount)
        {
            Z += amount;
            Z = Utils.Clamp(Z, MinZ, MaxZ);
            UpdateMatrices();
        }
        
        public void ResetZ()
        {
            Z = BaseZ;
        }
        
        #region MoveCam
        
        public void Move(Vector2 amount)
        {
            Position += amount;
        }
        
        public void MoveTo(Vector2 position)
        {
            Position = position;
        }

        public void FollowPlayers()
        {
            var players = Data.Characters;
            var camPos = players.Aggregate(Vector2.Zero, (current, player) => current + player.Value.Position);
            camPos /= players.Count;
            
            Position = camPos;
        }

        #endregion
        
        #region Zooms

        private void SetZoom(int amount)
        {
            Zoom = amount;
            Zoom = Utils.Clamp(Zoom, MinZoom, MaxZoom);
            Z = BaseZ / Zoom;
        }
        public void ZoomIn()
        {
            SetZoom(Zoom + 1);
        }
        public void ZoomOut()
        {
            SetZoom(Zoom - 1);
        }
        #endregion
        
        #region GetExtents
        public void GetExtents(out float width, out float height)
        {
            height = GetHeightFromZ();
            width = height * _aspectRatio;
        }
        
        public void GetExtents(out float left, out float right, out float top, out float bottom)
        {
            GetExtents(out float width, out var height);
            left = Position.X - width * .5f;
            right = left + width;
            bottom = Position.Y - height * .5f;
            top = bottom + height;
        }
        
        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            GetExtents(out var left, out var right, out var top, out var bottom);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }
        #endregion
    }
}