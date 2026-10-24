using UnityEngine;

public class JugadorMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadHorizontal = 7f;
    public float fuerzaSalto = 12f;
    
    [Header("Gravedad y Sensaciones")]
    public float multiplicadorCaida = 2.5f; // Hace que la caída sea más pesada (estilo Mario)
    public float multiplicadorSaltoCorto = 2f; // Si suelta la tecla, cae más rápido

    [Header("Detección de Suelo")]
    public Transform comprobadorSuelo; // Un GameObject vacío (Hijo de la pelota) en la base de la pelota
    public float radioComprobador = 0.2f;
    public LayerMask capaSuelo; // Define qué es "Suelo" en Unity

    private Rigidbody2D rb;
    private bool estaEnElSuelo;

    private Vector2 posicionInicio;

    void Reaparecer()
    {
        transform.position = posicionInicio;
        rb.linearVelocity = Vector2.zero;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicionInicio = transform.position;
    }

    void Update()
    {
        // 1. COMPROBAR EL SUELO (Robado de Tarodev: en lugar de OnCollision, usamos físicas para detectar el suelo al instante)
        estaEnElSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioComprobador, capaSuelo);

        // 2. MOVIMIENTO HORIZONTAL
        float inputX = 0f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) inputX = 1f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) inputX = -1f;
        
        rb.linearVelocity = new Vector2(inputX * velocidadHorizontal, rb.linearVelocity.y);

        // 3. EL SALTO (Solo si tocamos el suelo)
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && estaEnElSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto); // Aquí usamos linearVelocity también para el salto por ser más preciso
        }

        // 4. GRAVEDAD MEJORADA (Variable Jump Height de Tarodev)
        // Si estamos cayendo, aplicamos más gravedad
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorCaida - 1) * Time.deltaTime;
        }

        if (transform.position.y < -10f)
        {
            Reaparecer();
        }
        
        // Si estamos subiendo pero soltamos la tecla de salto, aplicamos gravedad extra para un "salto corto"
        else if (rb.linearVelocity.y > 0 && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorSaltoCorto - 1) * Time.deltaTime;
        }

        
    }

    // Dibuja un círculo rojo en el editor para que veas dónde está el detector de suelo
    private void OnDrawGizmos()
    {
        if (comprobadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(comprobadorSuelo.position, radioComprobador);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trampa"))
        {
            Reaparecer();
        }
        

    }
}