-- local common libs
local require = require
local core    = require("apisix.core")

-- module define
local plugin_name = "custom-header"

-- plugin schema
local plugin_schema = {
    type = "object",
    properties = {},
    required = {},
}

local _M = {
    version  = 0.1,            -- plugin version
    priority = 0,              -- the priority of this plugin will be 0
    name     = plugin_name,    -- plugin name
    schema   = plugin_schema,  -- plugin schema
}


-- module interface for schema check
-- @param `conf` user defined conf data
-- @param `schema_type` defined in `apisix/core/schema.lua`
-- @return <boolean>
function _M.check_schema(conf, schema_type)
    return core.schema.check(plugin_schema, conf)
end


-- module interface for header_filter phase
function _M.header_filter(conf, ctx)

end


return _M