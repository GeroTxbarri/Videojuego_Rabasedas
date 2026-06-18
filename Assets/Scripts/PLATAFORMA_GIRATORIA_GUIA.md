# Plataforma Giratoria - Guía de Implementación

## 📋 Descripción

El script `PlataformaGiratoria.cs` implementa una plataforma que gira alrededor de un punto central. El jugador que esté sobre la plataforma se mueve automáticamente junto con ella.

## 🔧 Características principales:
- ✅ Rotación alrededor de un punto central
- ✅ Detección automática del jugador (sin necesidad de configurar capas)
- ✅ **El jugador puede moverse libremente sobre la plataforma** mientras es arrastrado por la rotación
- ✅ El jugador NO se cae cuando se queda quieto
- ✅ Gizmos visuales en el editor para ver la trayectoria
- ✅ Propiedades ajustables (punto central, radio, velocidad)

## 📐 Configuración Paso a Paso

### 1. **Crear la Plataforma en la Escena**

- En la jerarquía de Unity, crea un nuevo objeto 3D (por ejemplo, un Cube o una plataforma personalizada)
- Renómbralo a "PlataformaGiratoria"
- Asegúrate de que tiene un **Collider** (BoxCollider, MeshCollider, etc.)
- Si usas un Cube, elimina el componente Rigidbody (la plataforma NO debe ser dinámica)

### 2. **Agregar el Script**

- Selecciona el GameObject de la plataforma
- En el Inspector, haz clic en "Add Component"
- Busca y agrega el script `PlataformaGiratoria`

### 3. **Configurar los Parámetros**

En el Inspector, encontrarás estos parámetros:

| Parámetro | Descripción | Valor Recomendado |
|-----------|-------------|-------------------|
| **Punto Central** | Coordenada (X, Y, Z) alrededor de la cual gira la plataforma | (0, 0, 0) o en el centro de tu mapa |
| **Radio** | Distancia en metros desde el punto central | 5-10 metros |
| **Velocidad Rotación** | Velocidad de giro en grados por segundo | 45-90 grados/seg |

**Ejemplo de Configuración:**
```
Punto Central: (10, 0, 10)
Radio: 7
Velocidad Rotación: 60
```

### 4. **Configurar el Jugador** (si es necesario)

- El jugador debe tener un **Rigidbody** (ya lo tiene en tu proyecto)
- El Collider del jugador debe estar configurado correctamente
- **No necesitas asignar capas específicas** - el script detecta automáticamente cualquier objeto con Rigidbody que colisione

### 5. **Prueba en Juego**

1. Presiona Play en Unity
2. Mueve el jugador hacia la plataforma
3. El jugador debería detectarse automáticamente y empezar a rotar con ella

## 🎮 Cómo Funciona Internamente

```
1. Cada frame, el script calcula el ángulo actual
2. Calcula la nueva posición de la plataforma usando trigonometría
3. Si el jugador está sobre la plataforma:
   - Calcula la velocidad tangencial (perpendicular al radio)
   - Aplica esa velocidad al Rigidbody del jugador
   - El jugador mantiene su velocidad Y original (para no interfereir con la gravedad)
```

## 🔄 Fórmula de Movimiento

```
Posición = PuntoCentral + (cos(ángulo) * radio, 0, sen(ángulo) * radio)
Velocidad Tangencial = (velocidad angular) × (radio)
```

## ⚙️ Propiedades en Tiempo de Ejecución

Puedes cambiar estos valores desde otro script:

```csharp
PlataformaGiratoria plataforma = GetComponent<PlataformaGiratoria>();
plataforma.VelocidadRotacion = 90f;  // Aumentar velocidad
plataforma.Radio = 10f;              // Cambiar radio
plataforma.PuntoCentral = new Vector3(5, 0, 5);  // Cambiar centro
```

## 📍 Visualización en el Editor

Cuando no estás en modo Play, verás en el Editor:
- 🔴 **Punto rojo**: Centro de rotación
- 🟢 **Esfera verde**: Radio de la rotación
- 🟡 **Línea amarilla**: Trayectoria circular (aproximada)
- 🔵 **Esfera azul** (solo en Play): Posición actual de la plataforma

## 🐛 Solución de Problemas

### El jugador no se mueve con la plataforma
- ✓ Verifica que el Collider de la plataforma es **"Is Trigger" = FALSE**
- ✓ Verifica que el Rigidbody del jugador está configurado correctamente
- ✓ Asegúrate de que el jugador tiene el script `Movimiento_jugador` O está etiquetado con Tag "Player"

### El jugador se cae de la plataforma
- ✓ Aumenta el tamaño de la plataforma (más ancha)
- ✓ Disminuye la velocidad de rotación
- ✓ Verifica que el Collider de la plataforma está activo y tiene buen contacto

### El jugador no puede moverse sobre la plataforma
- ✓ Ahora el jugador PUEDE moverse mientras está sobre la plataforma
- ✓ La plataforma le "arrastra" en la dirección de rotación sin bloquear sus controles
- ✓ Si sigue sin poder moverse, revisa que el Rigidbody no esté congelado en posición

### La plataforma gira demasiado rápido/lento
- ✓ Ajusta "Velocidad Rotación" en el Inspector

## 💡 Variaciones Posibles

### Plataforma que sube/baja mientras gira
```csharp
// En ActualizarPosicionPlataforma(), agrega:
float altura = Mathf.Sin(anguloActual * Mathf.Deg2Rad) * 2f;
nuevaPosicion.y = altura;
```

### Cambiar dirección de rotación
```csharp
// En FixedUpdate(), cambia:
anguloActual -= velocidadRotacion * Time.fixedDeltaTime;  // En lugar de +=
```

### Rotación con aceleración/desaceleración
```csharp
// Agrega una variable aceleration y modifica velocidadRotacion en FixedUpdate()
```

## 📞 Integración con tu Juego

Si necesitas que la plataforma interactúe con otras mecánicas:

```csharp
// Desde otro script, accede a la plataforma así:
PlataformaGiratoria plat = GetComponent<PlataformaGiratoria>();

// O buscando en la escena:
PlataformaGiratoria plat = FindObjectOfType<PlataformaGiratoria>();
```

¡Listo! Tu plataforma giratoria está implementada y lista para usar. 🎉

---

## 📝 Cambios Realizados (v2)

Se arreglaron los siguientes problemas:

### ❌ Eliminado: "Capa Jugador"
- **Problema**: Requerías configurar capas específicas manualmente
- **Solución**: El script ahora detecta automáticamente cualquier objeto con `Rigidbody` que tenga el script `Movimiento_jugador` o Tag "Player"

### ✅ Arreglado: El jugador se caía/rechazaba
- **Problema**: La plataforma reemplazaba completamente la velocidad del jugador
- **Solución**: Ahora solo se aplica la velocidad tangencial de rotación, permitiendo que el jugador mantenga su propia velocidad vertical (gravedad) y pueda interactuar con controles

### ✅ Arreglado: El jugador no podía moverse sobre la plataforma
- **Problema**: Los controles del jugador eran ignorados
- **Solución**: El jugador ahora puede moverse libremente (WASD, etc.) mientras la plataforma lo arrastra en la dirección de rotación

### ✅ Arreglado: El jugador se caía cuando se quedaba quieto
- **Problema**: La falta de input hacía que el jugador cayera
- **Solución**: La velocidad tangencial de la plataforma se aplica constantemente, manteniendo al jugador en la trayectoria circular
