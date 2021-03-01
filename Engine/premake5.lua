project "DigBuild.Engine"
    kind "SharedLib"
    framework "net5.0"
    language "C#"
    csversion "9.0"
    enabledefaultcompileitems(true)
    allownullable(true)
    noframeworktag(true)
    targetdir "../bin/%{cfg.buildcfg}"
    objdir "../bin-int/%{cfg.buildcfg}"

    dependson {
		"DigBuild.Platform",
		"DigBuild.Platform.SourceGen"
	}
    links {
		"DigBuild.Platform"
	}
	analyzer {
		"DigBuild.Platform.SourceGen"
	}

    filter "configurations:Debug"
        defines { "DB_DEBUG" }
        symbols "On"
