#!/bin/bash

set -e

[[ ${#@} != 1 ]] && { echo "Usage: release-nexus.sh githubTag"; exit 1; }

github_tag=$1
echo "Github Tag: $github_tag"

project=${github_tag%-*}
root=$(dirname "$(readlink -f "$0")")/..
changelog="Detailed changelog available on Github. https://github.com/rfvgyhn/schedule-one-mods/releases/$github_tag"

case $project in
  DealsSummary)
    modId=825
    ;;
  CounterPriceButton)
    modId=830
    ;;
  *)
    echo "Unknown project '$project'"
    exit 1
esac

artifact=$(find "$root"/artifacts/ -name "*$project*.zip" -type f)
[[ "$artifact" =~ v([0-9]+\.[0-9]+\.[0-9]+) ]] && version="${BASH_REMATCH[1]}" || { echo "Couldn't parse version number"; exit 1; }

export UNEX_GAME=schedule1
export UNEX_MODID=$modId

latest_version=$(
    curl -s \
         -H "apiKey: $UNEX_APIKEY" \
         "https://api.nexusmods.com/v1/games/$UNEX_GAME/mods/$modId" \
    | jq -r '.version'
)

[[ "$(printf '%s\n' "$version" "$latest_version" | sort -V | head -n1)" = "$version" ]] &&
    { echo "Latest version on Nexus Mods '$latest_version' is equal to or newer than '$version'"; exit 1; }

dotnet unex check
dotnet unex upload $modId $artifact -v $version
dotnet changelog $version -c "$changelog"