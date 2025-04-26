#!/bin/bash

set -e

{ echo "No mod IDs yet"; exit 1; }

project=$1
release_notes_path=$2

root=$(dirname "$(readlink -f "$0")")/..
changelog=$(cat "$release_notes_path")
artifact=$(find "$root"/artifacts/ -name "*$project*.zip" -type f)
[[ "$artifact" =~ v([0-9]+\.[0-9]+\.[0-9]+) ]] && version="${BASH_REMATCH[1]}" || { echo "Couldn't parse version number"; exit 1; }
case $1 in
  DealsSummary)
    modId=123
    ;;
  CounterPriceButton)
    modId=321
    ;;
  *)
    echo "Unknown project '$1'"
    exit 1
esac

export UNEX_GAME=schedule1
export UNEX_MODID=$modId

dotnet unex check
dotnet unex upload $modId $artifact -v $version
dotnet changelog $version -c $changelog