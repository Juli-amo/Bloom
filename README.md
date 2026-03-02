# 🌸 Bloom: A Diegetic Farming & Storytelling Experience

**Bloom** is an atmospheric 2D storytelling experience set in a living, evolving ecosystem. As a gardener on a melancholic island, you navigate the delicate balance between nurturing nature and the relentless drive for technological progress.

---

## Vision & Design Philosophy

Unlike traditional farming simulators, **Bloom** shies away from economic optimization and focuses on **Systemic Storytelling**:

- **Narrative**  
  The story is conveyed through the world itself, visual shifts and environmental discoveries (**Diary Fragments**)..

- **Visual Symbiosis**  
  Hand-crafted pixel art with a focus on **Y-sorting** and depth creates a dense 2D environment.

---

## 🎮 Key Features

- **Dynamic Plant Growth**  
  Six unique plant species with multiple growth and decay phases.

- **Wandering Merchant**  
  A dynamic NPC system featuring a trade-based progression gate.

- **UI**  
  A modular inventory system (**Backpack & Combining**) utilizing custom **Drag & Drop** logic.

- **Interactive Lore**  
  Environmental triggers and time-delayed diary fragments that reveal the island’s history.

---

## 🚀 Getting Started (Installation)

To open and edit the project in Unity, follow these steps:

### Prerequisites
- Unity Hub installed  
- Unity Editor **2022.3 LTS** (or newer)

### Installation

1) **Clone or download the repository**
```bash
git clone https://github.com/your-username/bloom-game.git
```

2) **Add the project to Unity Hub**
- Open Unity Hub  
- Click **Add → Add project from disk**  
- Select the folder containing **ProjectSettings**

3) **First launch**
- Locate and open the scene **Startscene** under `Assets/Scenes/`  
- Press **Play** to start from the main menu

---

## 🕹️ Controls

| Key | Action |
|---|---|
| WASD / Arrow Keys | Character Movement |
| E | Interact (Plant, Water, Talk, Read) |
| B | Open/Close Backpack & Combining UI |
| Scroll / 1-5 | Hotbar Selection |
| Left Click | Drag & Drop Items in Inventory |
| R (Debug) | Emergency UI/Camera Reset (Development only) |

---

## 🏗️ Technical Architecture

The project uses a **Data-Driven Design** to ensure scalability:

- **ScriptableObjects**  
  All items, plants, and recipes are stored as assets, allowing balancing without code changes.

- **Universal Pickups**  
  A single modular prefab system that dynamically adapts its visuals and properties based on assigned item data.

- **Event-Driven UI**  
  The interface responds to events (e.g., `OnInventoryChanged`), optimizing performance by avoiding constant frame-by-frame queries.

---

## 🗂 Project Structure

```text
Bloom/
├── Assets/
│   ├── Art/                # Sprites, Tilesets & UI Graphics
│   ├── Audio/              # Music & Ambient Soundscapes
│   ├── Prefabs/            # Modular Objects (Plants, Player, Plots)
│   ├── Scenes/             # StartMenu & GameWorld
│   ├── Scripts/            # C# Logic (Core, Systems, Player, UI)
│   └── ScriptableObjects/  # Item Databases & Recipes
└── README.md
```

---

## 👥 The Team

This project was developed as part of the **Interactive Storytelling** module at **Düsseldorf University of Applied Sciences (HSD)**, Winter Semester **2025/26**.

- **Yunha Chang**: Tile Design | Plant Design | UI Design | Animations  
- **Dalia Salih**: Programming (C#) | Core Loops | Storytelling  
- **Venci Wang**: Tile Design | Tool Design | Crafting System | Animations  
- **Julia Moor**: Programming (C#) | Worldbuilding | Audio  
- **Instructor**: Prof. Kai Schröder  

© 2026 Team Bloom. For academic purposes only.
