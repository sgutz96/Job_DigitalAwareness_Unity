# 🤖 Proyecto Unity – Avatar Conversacional con IA, Animaciones y TTS

Este proyecto implementa un **avatar animado en Unity** capaz de:

* Responder mensajes mediante un modelo de lenguaje (Ollama)
* Hablar usando **Text‑to‑Speech** (Windows System.Speech)
* Ejecutar animaciones sincronizadas según la reacción generada por la IA
* Mostrar el texto de respuesta en pantalla
* Cargar y consultar información estructurada (dataset universitario)

---

## 📌 Características principales

### ✔ 1. **Chat con IA (Ollama)**

El sistema envía un texto al modelo y recibe:

* La respuesta en texto
* Una reacción (enum) para animar al personaje
* La duración estimada del habla

Existen dos controladores principales:

* `OllamaChatWithDataSet`
* `OllamaChatSanpachito`

Cada uno tiene su propio manager para conectar la IA con el avatar.

---

### ✔ 2. **Managers de interacción**

Clases:

* `Manager`
* `SanpachitoMager`

Ambos se encargan de:

* Enviar el texto escrito por el usuario
* Activar/desactivar el botón mientras la IA procesa
* Mostrar la respuesta en pantalla
* Llamar a la animación correspondiente

Incluyen un sistema de animación basado en `Animator.SetBool()` más un reseteo automático por corrutina.

---

### ✔ 3. **Animaciones sincronizadas por reacción**

Cuando la IA devuelve una reacción (`Reaction`), el manager ejecuta:

```csharp
animator.SetBool(reaction.ToString(), true);
StartCoroutine(AnimateCharacterResert(reaction.ToString(), duration));
```

Esto permite que cada reacción tenga su propia animación definida en Unity.

---

### ✔ 4. **Text To Speech (TTS)**

El script `TTS_PS` permite generar voz real en Windows usando PowerShell:

* Selección de voces instaladas
* Control de volumen
* Control de velocidad

Funciona creando un proceso de PowerShell que ejecuta `System.Speech.Synthesis`.

Incluye un atributo personalizado `[VoiceSelector]` para mostrar un dropdown en el Inspector.

---

### ✔ 5. **Dataset estructurado: Universidades y Programas**

El proyecto incluye una estructura completa para cargar y consultar información:

* Universidades
* Programas académicos
* Acreditaciones
* Costos
* Planes de estudio

Estas clases están listas para la serialización desde JSON.

---

## 🧱 Estructura del Proyecto

```
📁 Scripts/
│
├── Manager.cs
├── SanpachitoMager.cs
│
├── TTS_PS.cs
│
├── UniversidadData.cs
├── Universidad.cs
└── ...

📁 Resources/
│   └── dataset_universidades.json

📁 Animations/
📁 Prefabs/
📁 Scenes/
```

---

## ⚙ Cómo funciona el flujo completo

1. El usuario escribe un mensaje
2. El manager lo envía al modelo Ollama
3. El modelo responde con:

   * texto
   * reacción (animación)
   * duración del habla
4. Se muestra el texto en pantalla
5. Se reproduce la animación
6. El TTS convierte la respuesta en voz

---

## 📦 Instalación paso a paso

### 1️⃣ Instalar **Ollama**

1. Descargar Ollama desde su página oficial
2. Instalar según tu sistema operativo
3. Verificar instalación ejecutando:

   ```bash
   ollama --version
   ```
4. Listar los modelos instalados:

   ```bash
   ollama list
   ```
5. Instalar/eliminar/ejecutar modelos recomendados:

   ```bash
   ollama run mistral        # Modelo rápido y liviano
   ollama run gemma          # Modelo de Google
   ollama run phi            # Modelo eficiente de Microsoft
   ollama run codellama      # Modelo para programación
   ollama rm llama2          # Eliminar un modelo
   ```

### 📘 Explicación rápida de los comandos

* **ollama --version**: Verifica que Ollama esté instalado correctamente.
* **ollama list**: Muestra todos los modelos disponibles en tu sistema.
* **ollama run <modelo>**: Ejecuta un modelo de lenguaje en modo consola.
* **ollama rm <modelo>**: Elimina un modelo que ya no necesites.
* **mistral / gemma / phi / codellama**: Modelos recomendados según el uso.

---

### 2️⃣ Instalar voces TTS en Windows (requerido por `TTS_PS`)**

1. Descargar Ollama desde su página oficial
2. Instalar según tu sistema operativo
3. Verificar instalación ejecutando:

   ```bash
   ollama --version
   ```
4. Instalar el modelo que usarás, por ejemplo:

   ```bash
   ollama pull llama3
   ```

---

### 2️⃣ Instalar voces TTS en Windows (requerido por `TTS_PS`)

1. Abrir **Configuración de Windows** → *Hora e idioma*
2. Entrar en **Voz**
3. Instalar voces adicionales si las necesitas
4. Asegurarte de tener al menos una voz instalada

---

### 3️⃣ Configurar Unity

1. Instalar Unity 2021 o superior
2. Crear un proyecto **3D** o **URP**
3. Copiar los scripts del proyecto en tu carpeta `Assets/Scripts`
4. Instalar TMP si Unity lo solicita

---

### 4️⃣ Configurar la escena

1. Crear un GameObject vacío y agregar:

   * `Manager` o `SanpachitoMager`
2. Arrastrar tus elementos a los campos del inspector:

   * `TMP_InputField`
   * `Button`
   * `Animator`
   * `ChatRespuesta` (TMP_Text)
3. Asignar el avatar con Animator y sus animaciones

---

### 5️⃣ Colocar el dataset (si se usa)

1. Guardar el JSON en `Resources`
2. Asegurar que los nombres coincidan con los scripts

---

### 6️⃣ Probar la comunicación con Ollama

Puedes hacer una prueba rápida llamando:

```csharp
Ollama.SendPrompt("Hola");
```

Si responde, todo está conectado correctamente.

---

## ▶ Cómo ejecutar

1. Abrir el proyecto en Unity
2. Asegurar que Windows tenga voces instaladas
3. Asignar en la escena:

   * Los managers
   * El input field
   * Botón enviar
   * Animator del avatar
4. Probar enviando un mensaje

---

## 🧩 Dependencias

* Unity 2021 o superior
* Windows (para el TTS con PowerShell)
* Modelo configurado en Ollama

---

## 🚀 Mejoras futuras sugeridas

* Sistema de lip‑sync automático
* Mezcla de animaciones (Animator Layers)
* Integración con visemas
* Sustituir PowerShell por un TTS nativo en C#
* Streaming de voz en tiempo real

---

## 📄 Licencia

Proyecto de uso libre para pruebas, investigación o desarrollo educativo.

---

## ✨ Autor

Proyecto desarrollado con apoyo de ChatGPT para estructurar IA + Animación + TTS en Unity.
