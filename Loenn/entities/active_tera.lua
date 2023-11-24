local teraEnum = require("mods").requireFromPlugin("libraries.enum")
local activeTera = {}

activeTera.name = "TeraHelper/activeTera"
activeTera.depth = 0
activeTera.texture = "TeraHelper/objects/tera/Block/Any"
activeTera.placements = {
	name = "Active Tera",
    data = {
        active = true,
        tera = "Normal",
    }
}

activeTera.fieldInformation = {
    tera = {
        fieldType = "anything",
        options = teraEnum.teraType,
        editable = false
    }
}

return activeTera