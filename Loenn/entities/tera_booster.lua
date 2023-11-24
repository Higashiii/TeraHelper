local drawableSprite = require("structs.drawable_sprite")
local teraEnum = require("mods").requireFromPlugin("libraries.enum")

local teraBooster = {}

teraBooster.name = "TeraHelper/teraBooster"
teraBooster.depth = -8500
teraBooster.placements = {
    {
        name = "Green Tera Booster",
        data = {
            red = false,
            tera = "Normal",
        }
    },
    {
        name = "Red Tera Booster",
        data = {
            red = true,
            tera = "Normal",
        }
    }
}

teraBooster.fieldInformation = {
    tera = {
        options = teraEnum.teraType,
        editable = false
    }
}
function teraBooster.sprite(room, entity, viewport)
    local x, y = entity.x or 0, entity.y or 0
    local red = entity.red
    local text
    if red then
        text = "objects/booster/boosterRed00"
    else
        text = "objects/booster/booster00"
    end
    local sprites = { drawableSprite.fromTexture(text, { x = x, y = y }) }
    local tera = entity.tera or "Normal"
    local texture = "TeraHelper/objects/tera/Block/" .. tera
    table.insert(sprites, drawableSprite.fromTexture(texture, { x = x, y = y }))
    return sprites
end

return teraBooster