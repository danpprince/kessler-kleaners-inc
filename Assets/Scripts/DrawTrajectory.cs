using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DrawTrajectory : MonoBehaviour
{
    // For reference:
    // https://github.com/ToughNutToCrack/TrajectoryPrediction/blob/master/Assets/Scripts/PredictionManager.cs

    public int maxIterations = 100;
    public float physicsStepMultiplier = 1.0f;
    public float collisionUpdateRateSeconds = 1.0f;
    public List<GameObject> obstacles;
    public GameObject subject;
    public KatamariMovement km;
    public LineRenderer lineRenderer;

    private PhysicsScene currentPhysicsScene;
    private PhysicsScene predictionPhysicsScene;
    private Scene predictionScene;
    private float timeSinceLastUpdateSeconds = 0.0f;
    

    void Start()
    {
        Physics.autoSimulation = false;

        Scene currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = 
            new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();


        CopyAllObstacles();
        PredictKatamariPath();
    }

    public void CopyAllObstacles()
    {
        foreach (GameObject o in obstacles)
        {
            if (o.GetComponent<Collider>() == null) continue;

            GameObject fakeObstacle = 
                Instantiate(o, o.transform.position, o.transform.rotation);
            SceneManager.MoveGameObjectToScene(fakeObstacle, predictionScene);
        }
    }

    void Update()
    {
        timeSinceLastUpdateSeconds += Time.deltaTime;

        if (timeSinceLastUpdateSeconds > collisionUpdateRateSeconds && subject.GetComponent<KatamariMovement>().isGolfHitMode)
        {
            PredictKatamariPath();
            timeSinceLastUpdateSeconds = 0.0f;
        }
    }

    void FixedUpdate()
    {
        // Run the actual scene's physics
        if (currentPhysicsScene.IsValid())
        {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    void PredictKatamariPath()
    {
        if (!currentPhysicsScene.IsValid() || !predictionPhysicsScene.IsValid())
        {
            Debug.LogError("Invalid physics scene in DrawTrajectory");
            return;
        }

        GameObject predictionSubject = 
            Instantiate(subject, subject.transform.position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(predictionSubject, predictionScene);

        Rigidbody subjectRigidbody = subject.GetComponent<Rigidbody>();
        Rigidbody predictionRigidbody = predictionSubject.GetComponent<Rigidbody>();

        // Copy the Rigidbody state from the subject to the prediction
        predictionRigidbody.velocity = subjectRigidbody.velocity;
        predictionRigidbody.angularVelocity = subjectRigidbody.angularVelocity;
        predictionRigidbody.inertiaTensor = subjectRigidbody.inertiaTensor;
        predictionRigidbody.inertiaTensorRotation = subjectRigidbody.inertiaTensorRotation;

        predictionRigidbody.useGravity = true;
        
        lineRenderer.positionCount = maxIterations;
        
        bool isGolfHitMode = km.IsGolfHitMode();
        ForceMode forceMode = isGolfHitMode ? ForceMode.Impulse : ForceMode.Force;
        for (int i = 0; i < maxIterations; i++)
        {
            lineRenderer.SetPosition(i, predictionSubject.transform.position);

            float accelerateFuelUsed = 1.0f;

            bool isFirstHitIteration = isGolfHitMode && i == 0;
            if (isFirstHitIteration || !isGolfHitMode)
            {
                Vector3 hitVector = km.CalculateHitVector(accelerateFuelUsed);
                predictionRigidbody.AddForce(hitVector, forceMode);

                Vector3 rollVector = km.CalculateRollVector(accelerateFuelUsed);
                predictionRigidbody.AddTorque(rollVector, forceMode);
            }

            // predictionRigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);

            predictionPhysicsScene.Simulate(physicsStepMultiplier * Time.fixedDeltaTime);
        }

        Destroy(predictionSubject);
    }
}
