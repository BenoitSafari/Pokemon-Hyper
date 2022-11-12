# Game Map Design
Maps are designed using [Tilemaps](https://docs.godotengine.org/en/stable/tutorials/2d/using_tilemaps.html?highlight=tilemaps#using-tilemaps) nodes.

## Index rules
|Z-Index||Content|
|---|---|---|
|**3...**|||
|**2**| ![hero.png](/docs/img/hero.png)|Player level.|
|**1**||Player-sized object level.|
|**0**||Ground level.|
|**-1**||Water level.|
|**-2...**|||