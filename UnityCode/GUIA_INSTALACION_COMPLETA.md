# 🎮 GUÍA DE INSTALACIÓN COMPLETA - FOOTBALL MASTER FIFA 2025

## 👶 **PARA PRINCIPIANTES ABSOLUTOS (Como explicar a un niño de 10 años)**

### 🎯 **PASO 1: DESCARGAR UNITY**

1. **Abre tu navegador web** (Chrome, Firefox, etc.)
2. **Ve a**: `https://unity.com/download`
3. **Haz clic en "Download Unity Hub"** (es gratis)
4. **Descarga e instala Unity Hub** (como instalar cualquier programa)
5. **Abre Unity Hub**
6. **Haz clic en "Installs"** (en el lado izquierdo)
7. **Haz clic en "Install Editor"** (botón azul)
8. **Selecciona "Unity 2021.3 LTS"** (la versión estable)
9. **Haz clic en "Next"**
10. **Selecciona estas opciones:**
    - ✅ Android Build Support
    - ✅ iOS Build Support (si tienes Mac)
    - ✅ Visual Studio Community (para escribir código)
11. **Haz clic en "Install"** y espera (puede tardar 1-2 horas)

### 🎯 **PASO 2: CREAR TU PROYECTO**

1. **Abre Unity Hub**
2. **Haz clic en "Projects"** (lado izquierdo)
3. **Haz clic en "New Project"** (botón azul)
4. **Selecciona "3D"** (template)
5. **Nombre del proyecto**: `FootballMaster`
6. **Ubicación**: Elige una carpeta fácil de encontrar
7. **Haz clic en "Create Project"**
8. **Espera a que Unity se abra** (puede tardar 5-10 minutos la primera vez)

### 🎯 **PASO 3: CONFIGURAR UNITY (MUY IMPORTANTE)**

Cuando Unity se abra, verás una pantalla grande. Ahora vas a configurar todo:

#### A) **Configurar Layers (Capas)**
1. **Busca "Layers"** en la parte superior derecha
2. **Haz clic en "Layers" > "Edit Layers"**
3. **En "User Layer 8"** escribe: `Ground`
4. **En "User Layer 9"** escribe: `Player`
5. **En "User Layer 10"** escribe: `Ball`
6. **En "User Layer 11"** escribe: `HomeTeam`
7. **En "User Layer 12"** escribe: `AwayTeam`
8. **En "User Layer 13"** escribe: `UI`
9. **En "User Layer 14"** escribe: `Minimap`

#### B) **Configurar Tags (Etiquetas)**
1. **Busca "Tags"** en la parte superior derecha
2. **Haz clic en "Tags" > "Edit Tags"**
3. **Haz clic en el "+" y agrega estos tags:**
   - `Ball`
   - `Player`
   - `Goal`
   - `Ground`
   - `Post`
   - `Crossbar`
   - `Grass`
   - `Dirt`
   - `Concrete`
   - `Boundary`

### 🎯 **PASO 4: CREAR CARPETAS PARA TUS ARCHIVOS**

1. **Mira la parte inferior izquierda** (ventana "Project")
2. **Haz clic derecho en "Assets"**
3. **Selecciona "Create > Folder"**
4. **Crea estas carpetas exactamente así:**
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
   │   ├── ConfigurationSystem/
   │   └── AutoSetup/
   ├── Materials/
   ├── Textures/
   ├── Audio/
   ├── Prefabs/
   └── Scenes/
   ```

### 🎯 **PASO 5: COPIAR LOS SCRIPTS**

Ahora vas a copiar todos los archivos de código que te di:

#### A) **Scripts de Control:**
1. **Ve a la carpeta** `Assets/Scripts/TouchControlSystem/`
2. **Haz clic derecho > Create > C# Script**
3. **Nombra el script** `TouchControlManager_120fps`
4. **Haz doble clic** en el script para abrirlo
5. **Borra todo el contenido** que aparece
6. **Copia y pega** el contenido del archivo `TouchControlManager_120fps.cs`
7. **Guarda el archivo** (Ctrl+S)

#### B) **Scripts de Jugadores:**
1. **Ve a la carpeta** `Assets/Scripts/PlayerSystem/`
2. **Crea estos scripts y copia el contenido:**
   - `PlayerController_120fps.cs`
   - `PlayerData.cs`
   - `PlayerAI.cs`

#### C) **Scripts de Balón:**
1. **Ve a la carpeta** `Assets/Scripts/BallPhysics/`
2. **Crea estos scripts:**
   - `BallController_120fps.cs`
   - `TrajectoryPredictor.cs`

#### D) **Scripts de Juego:**
1. **Ve a la carpeta** `Assets/Scripts/GameplayMechanics/`
2. **Crea estos scripts:**
   - `GameManager_120fps.cs`
   - `CameraController_120fps.cs`
   - `GoalDetector.cs`
   - `PlayerStats.cs`

#### E) **Scripts de Equipos:**
1. **Ve a la carpeta** `Assets/Scripts/TeamsAndLeagues/`
2. **Crea estos scripts:**
   - `TeamData.cs`
   - `LeagueManager.cs`
   - `LeagueData.cs`

#### F) **Scripts de Setup Automático:**
1. **Ve a la carpeta** `Assets/Scripts/AutoSetup/`
2. **Crea estos scripts:**
   - `FieldGenerator.cs`
   - `PlayerGenerator.cs`
   - `GameSetup.cs`

### 🎯 **PASO 6: GENERAR EL CAMPO AUTOMÁTICAMENTE**

¡Ahora viene la parte mágica! Vas a crear tu campo de fútbol automáticamente:

1. **En la ventana "Hierarchy"** (lado izquierdo), haz clic derecho
2. **Selecciona "Create Empty"**
3. **Nombra el objeto** `Field Generator`
4. **Con "Field Generator" seleccionado**, ve a la ventana "Inspector" (lado derecho)
5. **Haz clic en "Add Component"**
6. **Busca "Field Generator"** y selecciónalo
7. **En el Inspector, haz clic en "Generate Field"**
8. **¡MAGIA!** Tu campo de fútbol aparecerá automáticamente

### 🎯 **PASO 7: GENERAR JUGADORES AUTOMÁTICAMENTE**

1. **Haz clic derecho en "Hierarchy"**
2. **Selecciona "Create Empty"**
3. **Nombra el objeto** `Player Generator`
4. **Agrega el componente "Player Generator"**
5. **En el Inspector, haz clic en "Generate All Players"**
6. **¡MAGIA!** Aparecerán 22 jugadores y un balón automáticamente

### 🎯 **PASO 8: CONFIGURAR NAVMESH (PARA QUE LOS JUGADORES CAMINEN)**

1. **Ve a "Window > AI > Navigation"**
2. **Selecciona tu campo de fútbol** en la Hierarchy
3. **En la ventana Navigation, marca "Navigation Static"**
4. **Haz clic en "Bake"**
5. **Espera a que termine** (aparecerá un área azul en el campo)

### 🎯 **PASO 9: CONFIGURAR SETUP AUTOMÁTICO**

1. **Haz clic derecho en "Hierarchy"**
2. **Selecciona "Create Empty"**
3. **Nombra el objeto** `Game Setup`
4. **Agrega el componente "Game Setup"**
5. **En el Inspector, haz clic en "Setup Complete Game"**
6. **¡MAGIA!** Se configurarán automáticamente:
   - Cámara del juego
   - Controles táctiles
   - Interfaz de usuario
   - Sonidos
   - Iluminación
   - Gestores del juego

### 🎯 **PASO 10: CONFIGURAR PARA MÓVIL**

#### A) **Configurar Build Settings:**
1. **Ve a "File > Build Settings"**
2. **Selecciona "Android"** (o iOS si tienes Mac)
3. **Haz clic en "Switch Platform"**
4. **Espera a que termine** (puede tardar 10-15 minutos)

#### B) **Configurar Player Settings:**
1. **En Build Settings, haz clic en "Player Settings"**
2. **Configura estos valores:**
   - **Company Name:** Tu nombre
   - **Product Name:** Football Master
   - **Package Name:** com.tunombre.footballmaster
   - **Target API Level:** 31 (Android)
   - **Minimum API Level:** 26 (Android)
   - **Scripting Backend:** IL2CPP
   - **Architecture:** ARM64

### 🎯 **PASO 11: PROBAR TU JUEGO**

1. **Haz clic en el botón "Play"** (triángulo en la parte superior)
2. **¡Tu juego debería funcionar!**
3. **Para controles:**
   - **Clic izquierdo:** Pase corto
   - **Arrastra:** Mover jugador
   - **Swipe rápido:** Disparo
   - **Doble clic:** Reset de cámara

### 🎯 **PASO 12: COMPILAR PARA MÓVIL**

#### A) **Para Android:**
1. **Conecta tu teléfono Android** por USB
2. **Activa "Developer Options"** en tu teléfono
3. **Activa "USB Debugging"**
4. **En Unity, ve a "File > Build Settings"**
5. **Haz clic en "Build and Run"**
6. **Guarda el archivo APK** donde quieras
7. **¡El juego se instalará automáticamente en tu teléfono!**

#### B) **Para iOS:**
1. **Necesitas una Mac** y una cuenta de Apple Developer
2. **En Unity, ve a "File > Build Settings"**
3. **Selecciona "iOS"**
4. **Haz clic en "Build"**
5. **Abre el proyecto en Xcode**
6. **Compila desde Xcode**

---

## 🎮 **RECURSOS ADICIONALES GRATIS**

### 🎵 **Música y Sonidos (Gratis):**
- **Freesound.org** - Efectos de sonido
- **Zapsplat.com** - Música de fondo
- **Mixkit.co** - Música libre de copyright

### 🎨 **Texturas y Materiales (Gratis):**
- **Textures.com** - Texturas de césped
- **Polyhaven.com** - Materiales PBR
- **Freepik.com** - Texturas de fútbol

### 👥 **Modelos 3D (Gratis):**
- **Mixamo.com** - Personajes y animaciones
- **Sketchfab.com** - Modelos 3D gratuitos
- **OpenGameArt.org** - Assets de juegos

---

## 🚨 **PROBLEMAS COMUNES Y SOLUCIONES**

### ❌ **Error: "Script missing"**
**Solución:** Verifica que copiaste bien el código y que el nombre del archivo coincida exactamente.

### ❌ **Error: "NavMesh not found"**
**Solución:** Asegúrate de haber bakeado el NavMesh (Paso 8).

### ❌ **Los jugadores no se mueven**
**Solución:** Verifica que el NavMesh esté bakeado y que los jugadores tengan el componente NavMeshAgent.

### ❌ **El juego va lento**
**Solución:** Ve a "Edit > Project Settings > Quality" y selecciona "Low" o "Medium".

### ❌ **No hay sonido**
**Solución:** Asegúrate de que el AudioManager esté configurado y que los clips de audio estén asignados.

### ❌ **El balón no rebota**
**Solución:** Verifica que el BallController esté asignado y que tenga un Rigidbody.

---

## 🏆 **¡FELICITACIONES!**

¡Has creado tu propio juego de fútbol profesional! Ahora tienes:

- ✅ **Campo de fútbol completo** con líneas y porterías
- ✅ **22 jugadores** con IA inteligente
- ✅ **Física realista** del balón con curvas
- ✅ **Controles táctiles** optimizados
- ✅ **Sistema de goles** automático
- ✅ **Cámara inteligente** que sigue la acción
- ✅ **Optimización 120fps** para móviles
- ✅ **Interfaz de usuario** profesional

## 🎯 **PRÓXIMOS PASOS OPCIONALES**

1. **Personaliza equipos** editando los PlayerData
2. **Agrega más animaciones** desde Mixamo
3. **Mejora los gráficos** con texturas profesionales
4. **Añade más sonidos** para mayor inmersión
5. **Crea un menú principal** más elaborado
6. **Implementa multijugador** online

---

**¡Tu Football Master FIFA 2025 está listo para conquistar el mundo! 🏆⚽🎮**

*Recuerda: Si tienes algún problema, revisa esta guía paso a paso. Todo está diseñado para funcionar automáticamente.*