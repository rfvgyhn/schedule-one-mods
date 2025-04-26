#!/bin/bash

set -e

root=$(dirname "$(readlink -f "$0")")/..
source "$root/ci/projects.sh"

version_suffix="$1"
target=win-x64

for project in "${default_projects[@]}"; do
    version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" "src/ScheduleOneMods.$project/ScheduleOneMods.$project.csproj")
    release_name="schedule-one-mods_${project}_v${version}-${version_suffix}"
    
    "$root/ci/publish.sh" $project $target $release_name Release "$1"
done