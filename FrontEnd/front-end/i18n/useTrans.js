import { useRouter } from "next/router";

import en from "./locales/en";
import vi from "./locales/vi";

const useTrans = () => {
  const { locale } = useRouter();
  let trans = vi;
  switch (locale) {
    case "vi":
      trans = vi;
      break;
    case "en":
      trans = en;
      break;
    default:
      trans = vi;
  }
};

export default useTrans;
