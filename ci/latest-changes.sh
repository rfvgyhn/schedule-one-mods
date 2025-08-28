#!/bin/bash
set -e
changelog="$(dirname "$(readlink -f "$0")")/../CHANGELOG.md"
project=$1

tail -n +2 "$changelog" |                              # Remove header
  sed -n "/^## \[$project/,/^##[^#]\|\[unreleased/p" | # Extract from first $project header to either next header or link section
  sed '1d' |                          # Remove release heading
  sed '/./,$!d' |                     # Remove whitespace lines
  sed '/^##[^#]\|\[unreleased\]:/q' | # Stop at next release heading or link section
  sed '$ d'                           # Remove last line