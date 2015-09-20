# 0.6.1

- Add new stage type: Transliterate.

# 0.6.0

- Stage types can now be mixed, which (among other things) allows applying a 
  Match stage to the result of one or more Replace stages. An odd number of
  stages is now allowed and makes the last stage default to Match mode.
- #21: Improve defaults for Silent mode. One can now choose between making
  individual stages Silent (or not) or entire loops. By default only the
  last, outermost loop is not Silent, which means you always get only a
  single output regardless of whether the last stage is looped.

# 0.5.0

First numbered version. Notable changes over previous version:

- #19: Intermediate stages are silent by default.
- #15: Stages can be grouped into loops.