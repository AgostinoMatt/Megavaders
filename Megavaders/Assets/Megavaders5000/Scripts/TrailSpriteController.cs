using UnityEngine;
using System.Collections;

/// <summary>
/// Trail sprite controller - simply reduce the size / opacity to a certain threshold, then despawn
/// </summary>
public class TrailSpriteController : MonoBehaviour 
{
    [Tooltip("Speed of scale reduction.")]
    [SerializeField]
    float ScaleDownSpeed = 0.2f;

    [Tooltip("Speed of sprite fade-out.")]
    [SerializeField]
    float FadeSpeed = 2.5f;

    SpriteRenderer sprite;
	Vector3 originalScale;
	float originalAlpha;

	void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		originalScale = transform.localScale;
		originalAlpha = sprite.color.a;

	}

	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 scale = transform.localScale;

        scale.x -= (ScaleDownSpeed * Time.deltaTime);
        scale.y -= (ScaleDownSpeed * Time.deltaTime);

        transform.localScale = scale;

        Color a = sprite.color;
        a.a -= FadeSpeed * Time.deltaTime;
        sprite.color = a;

		if (a.a < 0.01f) 
		{
			// Despawn this game
			RFLib.RFObjectPool.Despawn (gameObject);
			transform.localScale = originalScale;
			a.a = originalAlpha;
			sprite.color = a;
		}


	}
}
