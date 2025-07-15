# 🚀 Guía de Inicio Rápido - Football Master

## ⚡ Configuración en 10 Pasos

### 1. 📁 Crear Proyecto Unity
```
- Abrir Unity Hub
- Nuevo Proyecto 3D
- Nombre: "FootballMaster"
- Versión: Unity 2021.3 LTS o superior
```

### 2. 📋 Configurar Capas y Tags

**Capas (Layers):**
```
8. Ground
9. Player  
10. Ball
11. HomeTeam
12. AwayTeam
13. UI
14. Minimap
```

**Tags:**
```
- Ball
- Player
- Goal
- Ground
- Post
- Crossbar
- Grass
- Dirt
- Concrete
```

### 3. 📂 Estructura de Carpetas
```
Assets/
├── Scripts/
│   ├── TouchControlSystem/
│   ├── BallPhysics/
│   ├── PlayerSystem/
│   ├── GameplayMechanics/
│   ├── TeamsAndLeagues/
│   ├── AudioSystem/
│   ├── UISystem/
│   ├── SaveSystem/
│   ├── MultiplayerSystem/
│   ├── EffectsSystem/
│   ├── AchievementSystem/
│   ├── TutorialSystem/
│   └── ConfigurationSystem/
├── Prefabs/
├── Materials/
├── Audio/
├── Textures/
└── Scenes/
```

### 4. 🎮 Managers Principales
Crear GameObjects vacíos con estos nombres y scripts:

```csharp
// GameObject: GameManager
- GameManager.cs
- AudioSource (para sonidos del juego)

// GameObject: AudioManager  
- AudioManager.cs
- 4 AudioSources (music, sfx, crowd, commentary)

// GameObject: NetworkManager
- NetworkManager.cs

// GameObject: EffectsManager
- EffectsManager.cs

// GameObject: AchievementManager
- AchievementManager.cs

// GameObject: TutorialManager
- TutorialManager.cs

// GameObject: GameData
- GameData.cs
```

### 5. ⚽ Configurar Balón
```csharp
// GameObject: Ball
- Esfera 3D (scale: 0.22, 0.22, 0.22)
- Rigidbody (masa: 0.45, drag: 0.05)
- SphereCollider (radius: 0.11)
- BallController.cs
- TrajectoryPredictor.cs
- TrailRenderer (para efecto de estela)
- Tag: "Ball"
- Layer: "Ball"
```

### 6. 👥 Configurar Jugadores
```csharp
// GameObject: Player
- Modelo 3D de jugador (o cápsula temporal)
- NavMeshAgent
- CapsuleCollider
- Rigidbody
- PlayerController.cs
- PlayerAI.cs
- PlayerStats.cs
- FootstepAudio.cs
- Tag: "Player"
- Layer: "Player"
```

### 7. 🏟️ Configurar Campo
```csharp
// GameObject: Field
- Plane (scale: 10, 1, 15) - Campo de fútbol
- MeshCollider
- Material verde
- Tag: "Ground"
- Layer: "Ground"

// GameObject: Goal (x2)
- Crear porterías en cada extremo
- GoalDetector.cs en área de gol
- Tag: "Goal"
```

### 8. 📱 Configurar UI
```csharp
// Canvas Principal
- Canvas (Screen Space - Overlay)
- CanvasScaler
- GraphicRaycaster
- MainMenuUI.cs o GameplayUI.cs

// EventSystem
- EventSystem
- StandaloneInputModule
```

### 9. 🎵 Configurar Audio
```csharp
// En AudioManager:
- Asignar AudioClips
- Configurar volúmenes
- Verificar Audio Mixer Groups
```

### 10. 🎯 Configurar Controles
```csharp
// GameObject: TouchControls
- TouchControlManager.cs
- TrickDetector.cs
- Asignar referencias a Player y Ball
```

## 🔧 Configuración Rápida de Build

### Android
```csharp
File > Build Settings
- Platform: Android
- Architecture: ARM64
- Minimum API Level: 23
- Target API Level: 30
- Scripting Backend: IL2CPP
```

### iOS
```csharp
File > Build Settings
- Platform: iOS
- Architecture: ARM64
- Minimum iOS Version: 12.0
- Scripting Backend: IL2CPP
```

## 📋 Checklist de Configuración

- [ ] Proyecto Unity creado
- [ ] Capas y tags configurados
- [ ] Estructura de carpetas creada
- [ ] Scripts copiados a carpetas correctas
- [ ] Managers principales creados
- [ ] Balón configurado con física
- [ ] Jugadores con NavMesh y IA
- [ ] Campo de fútbol básico
- [ ] UI Canvas configurado
- [ ] Audio Manager configurado
- [ ] Controles táctiles configurados
- [ ] Build settings configurados

## 🎮 Primeras Pruebas

### Test 1: Movimiento del Balón
```csharp
1. Play en Unity
2. Verificar que el balón tenga física
3. Mover con Transform si es necesario
4. Verificar BallController funciona
```

### Test 2: Controles Táctiles
```csharp
1. Compilar en dispositivo móvil
2. Probar gestos básicos
3. Verificar detección de trucos
4. Ajustar sensibilidad si es necesario
```

### Test 3: IA de Jugadores
```csharp
1. Colocar jugadores en el campo
2. Verificar que se mueven hacia el balón
3. Probar diferentes estados de IA
4. Ajustar NavMesh si es necesario
```

## 🚨 Problemas Comunes y Soluciones

### "Script Missing"
```csharp
Solución: Verificar que el script esté en la carpeta correcta
```

### "NavMesh not found"
```csharp
Solución: 
1. Window > AI > Navigation
2. Seleccionar el campo
3. Marcar como "Navigation Static"
4. Clic en "Bake"
```

### "Touch not detected"
```csharp
Solución: Verificar EventSystem en la escena
```

### "Audio not playing"
```csharp
Solución: Verificar que AudioManager tenga AudioSources asignados
```

## 🎯 Próximos Pasos

1. **Personalizar Equipos**: Crear ScriptableObjects para equipos
2. **Agregar Animaciones**: Configurar Animator para jugadores
3. **Mejorar Gráficos**: Agregar texturas y materiales
4. **Configurar Ligas**: Crear datos de equipos reales
5. **Testear Multijugador**: Configurar networking
6. **Optimizar**: Usar Object Pooling y optimizaciones

## 📞 Ayuda Adicional

- **Unity Documentation**: https://docs.unity3d.com/
- **NavMesh Tutorial**: https://unity.com/learn/tutorials/navmesh
- **Mobile Optimization**: https://unity.com/learn/tutorials/mobile-optimization

¡Ahora tienes todo listo para crear tu juego de fútbol! 🏆⚽