using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoidsController : MonoBehaviour
{
    [SerializeField] private Boid prefab;
    [SerializeField] private int amount;
    [SerializeField] private Slider alignment, separation, coherence;

    private Boid[] boids;

    private void Awake()
    {
        alignment.onValueChanged.AddListener((float f) => Boid.AlignmentAmount = f);
        separation.onValueChanged.AddListener((float f) => Boid.SeparationAmount = f);
        coherence.onValueChanged.AddListener((float f) => Boid.CoherenceAmount = f);
        boids = new Boid[amount];
        if (prefab)
        {
            for (int i = 0; i < amount; ++i)
            {
                Boid boid = Instantiate(prefab, transform);
                boids[i] = boid;
            }
        }
    }

    private void Update()
    {
        Boid currentBoid;
        for (int i = 0; i < boids.Length; i++)
        {
            currentBoid = boids[i];
            currentBoid.UpdateBoid();
        }
    }
}
