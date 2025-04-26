#!/bin/bash

set -e

{ echo "No mod IDs yet"; exit 1; }

[[ ${#@} != 2 ]] && { echo "Usage: release-nexus.sh projectName githubTag"; exit 1; }

project=$1
github_tag=$2

root=$(dirname "$(readlink -f "$0")")/..
artifact=$(find "$root"/artifacts/ -name "*$project*.zip" -type f)
changelog="Detailed changelog available on Github. https://github.com/rfvgyhn/schedule-one-mods/releases/$github_tag"
[[ "$artifact" =~ v([0-9]+\.[0-9]+\.[0-9]+) ]] && version="${BASH_REMATCH[1]}" || { echo "Couldn't parse version number"; exit 1; }
case $project in
  DealsSummary)
    modId=123
    ;;
  CounterPriceButton)
    modId=321
    ;;
  *)
    echo "Unknown project '$project'"
    exit 1
esac

export UNEX_GAME=schedule1
export UNEX_MODID=$modId

dotnet unex check
dotnet unex upload $modId $artifact -v $version
dotnet changelog $version -c "$changelog"