using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class LaserSegment : Node2D
    {
        private const int MaxReflections = 10;
        private const float LightSpeed = 32 * 200;
        private const float MaxBeamLength = 1000;

        private PackedScene _laserSegmentPrefab => GD.Load<PackedScene>("res://Prefabs/Elements/LaserSegment.tscn");

        private Node2D _beam => GetNode<Node2D>("%Beam");
        private Node2D _star => GetNode<Node2D>("%Star");
        private RayCast2D _rayCast => GetNode<RayCast2D>("%RayCast");

        private int _reflectionsRemaining = MaxReflections;
        private LaserSegment _reflectedSegment = null;

        private float _beamLength = 0;

        public override void _Process(float delta)
        {
            // Fade out as the laser runs out of reflections
            var color = _beam.Modulate;
            color.a = ((float)_reflectionsRemaining) / MaxReflections;
            color.a = Mathf.Pow(color.a, 1.5f);
            _beam.Modulate = color;
        }

        public override void _PhysicsProcess(float delta)
        {
            UpdateLightBeam(delta);
            UpdateReflection();
        }

        private void UpdateLightBeam(float delta)
        {
            if (_rayCast.IsColliding())
            {
                _beamLength = _rayCast
                    .GetCollisionPoint()
                    .DistanceTo(GlobalPosition);
            }

            _star.Visible = _rayCast.IsColliding();
            _star.GlobalPosition = _rayCast.GetCollisionPoint();

            _beam.Scale = new Vector2(_beamLength, 1);
            _beamLength = Mathf.MoveToward(_beamLength, MaxBeamLength, LightSpeed * delta);
            _rayCast.CastTo = new Vector2(_beamLength, 0);
        }

        private void UpdateReflection()
        {
            Vector2 hitPoint = _rayCast.GetCollisionPoint();
            Vector2 hitNormal = _rayCast.GetCollisionNormal();

            // Don't reflect if any of the conditions aren't met
            bool isColliding = _rayCast.IsColliding();
            bool canStillReflect = _reflectionsRemaining > 0;
            bool surfaceIsReflective = IsReflectiveSurface(_rayCast.GetCollider());
            bool normalIsValid = hitNormal != Vector2.Zero;
            // GetCollisionNormal() will return (0, 0) if the entire ray
            // is inside the the collision shape, even if IsColliding() returns
            // true.

            if (!(isColliding && normalIsValid && canStillReflect && surfaceIsReflective))
            {
                if (_reflectedSegment != null)
                {
                    RemoveChild(_reflectedSegment);
                    _reflectedSegment.QueueFree();
                    _reflectedSegment = null;
                }
                return;
            }

            // Add a new segment, if we haven't already
            if (_reflectedSegment == null)
            {
                _reflectedSegment = _laserSegmentPrefab.Instance<LaserSegment>();
                _reflectedSegment._reflectionsRemaining =_reflectionsRemaining - 1;
                AddChild(_reflectedSegment);
            }

            // Move the start of the child segment to the point where
            // this laser hit the wall.
            //
            // We need to move it ever-so-slightly away from the wall,
            // though.  If we move it _exactly_ to the collision point,
            // then the next segment will start out already colliding
            // with the mirror, causing it to (sometimes) reflect again
            // immediately(depending on how the Floating Point Gods
            // decided to round that day).
            _reflectedSegment.GlobalPosition = hitPoint + hitNormal * 0.01f;

            // Angle the child segment, depending on how it hit the mirror.
            Vector2 forward = new Vector2(
                Mathf.Cos(GlobalRotation),
                Mathf.Sin(GlobalRotation)
            );

            Vector2 reflected = forward.Reflect(hitNormal);
            _reflectedSegment.GlobalRotation = reflected.Angle() - Mathf.Deg2Rad(180);
        }

        private bool IsReflectiveSurface(Object obj)
        {
            return obj is Node node && node.IsInGroup("ReflectiveGroup");
        }
    }
}
