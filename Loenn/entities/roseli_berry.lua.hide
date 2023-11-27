local drawableSprite = require("structs.drawable_sprite")

local roseliBerry = {}

roseliBerry.name = "TeraHelper/roseliBerry"
roseliBerry.depth = 100
roseliBerry.placements = {
    name = "Roseli Berry",
}

-- Offset is from sprites.xml, not justifications
local offsetY = -10
local offsetX = -1
local texture = "TeraHelper/objects/tera/Goomy/berry"

function roseliBerry.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(texture, entity)
    sprite.y += offsetY
    sprite.x -= offsetX
    return sprite
end

return roseliBerry