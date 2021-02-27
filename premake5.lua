require "PlatformBindings/modules/digbuild"

workspace "DigBuildEngine"
    configurations { "Debug", "Release" }
    --startproject "DigBuildEngineTest"

    include "PlatformBindings/PlatformCPP"
    include "PlatformBindings/PlatformCS"
    include "PlatformBindings/PlatformSourceGen"
    include "Engine"
    --include "EngineTest"

project "DigBuildPlatformCPP"
    targetdir "bin/%{cfg.buildcfg}"
    objdir "bin-int/%{cfg.buildcfg}"
project "DigBuildPlatformCS"
    targetdir "bin/%{cfg.buildcfg}"
    objdir "bin-int/%{cfg.buildcfg}"
