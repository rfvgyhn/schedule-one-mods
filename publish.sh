#!/bin/bash

set -e

source ci/projects.sh

target=win-x64
artifacts_path=artifacts
projects=("${@:-${default_projects[@]}}")

dotnet restore -r $target
dotnet build -c Release --no-restore

for project in "${projects[@]}"; do
    version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" "src/ScheduleOneMods.$project/ScheduleOneMods.$project.csproj")
    release_name="schedule-one-mods_${project}_v${version}"
    release_path="$artifacts_path/$release_name"
    
    ci/publish.sh $project $target $release_name Release    
    cp README.md CHANGELOG.md "$release_path"
    
    pushd $artifacts_path
        rm -f "$release_name.zip" && zip -r "$release_name.zip" "$release_name"
    popd
    
    rm -r "$release_path"
done
