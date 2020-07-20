using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private const float minY = 0.3291666666f, maxY = 0.9125f;

    public static bool FollowTarget = false;
    public static Vector2 Target = Vector2.negativeInfinity;
    public static float SeparationAmount = 1, CoherenceAmount = 1, AlignmentAmount = 1;

    [SerializeField] private Vector3 baseRotation;
    [Range(0, 10)] [SerializeField] private float maxSpeed = 1f;

    [Range(.1f, .5f)] [SerializeField] private float maxForce = .03f;

    [Range(1, 10)] [SerializeField] private float neighborhoodRadius = 3f;

    private Vector2 acceleration;
    private Vector2 velocity;

    private Vector2 Position
    {
        get
        {
            return gameObject.transform.position;
        }
        set
        {
            gameObject.transform.position = value;
        }
    }

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        float randomDirection = Random.Range(0, 360);
        Vector3 euler = transform.eulerAngles;
        euler.z = randomDirection;
        transform.eulerAngles = euler;
        transform.Translate(transform.up * Random.Range(0, 10), Space.Self);
    }

    public void UpdateBoid()
    {
        if (FollowTarget)
        {
            SteerTowardsTarget();
        }
        else
        {
            var boidColliders = Physics2D.OverlapCircleAll(Position, neighborhoodRadius);
            var boids = boidColliders.Select(o => o.GetComponent<Boid>()).ToList();
            boids.Remove(this);

            Flock(boids);
        }
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        WrapAround();
    }

    private void Flock(IEnumerable<Boid> boids)
    {
        var alignment = Alignment(boids);
        var separation = Separation(boids);
        var cohesion = Cohesion(boids);

        acceleration = AlignmentAmount * alignment + CoherenceAmount * cohesion + SeparationAmount * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, maxSpeed);
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
    }

    private void SteerTowardsTarget()
    {
        acceleration = (Target - Position).normalized * maxSpeed;
    }

    private Vector2 Alignment(IEnumerable<Boid> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<Boid> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<Boid> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= neighborhoodRadius / 2);
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    private float DistanceTo(Boid boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    private void WrapAround()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(Position);
        if (pos.x < 0)
        {
            pos = new Vector3(1, pos.y, pos.z);
        }
        else if (pos.x >= 1)
        {
            pos = new Vector3(0, pos.y, pos.z);
        }
        if (pos.y < minY)
        {
            pos = new Vector3(pos.x, maxY, pos.z);
        }
        else if (pos.y >= maxY)
        {
            pos = new Vector3(pos.x, minY, pos.z);
        }
        Position = Camera.main.ViewportToWorldPoint(pos);
    }
}