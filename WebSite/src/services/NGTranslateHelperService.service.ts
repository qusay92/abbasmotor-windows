import { Inject, Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { LanguagesEnum } from "src/enum/LanguagesEnum";

@Injectable({
    providedIn: 'root',
  })

  export class NGTranslateHelperService {

    constructor(private translate: TranslateService) {

    }

    addTranslationJsonFile(fileName: string){
      
    }

    initialization() {
        this.addLangs();
        this.getCurrentLang();
        this.translate.setDefaultLang(LanguagesEnum[LanguagesEnum.Ar]);
        this.useCurrent()
      }


      private addLangs() {
        this.translate.addLangs([LanguagesEnum[LanguagesEnum.En],LanguagesEnum[LanguagesEnum.Ar]]);  
      }

      getCurrentLang() {
        if (localStorage.getItem("currantLang") != LanguagesEnum[LanguagesEnum.Ar]
          && localStorage.getItem("currantLang") != LanguagesEnum[LanguagesEnum.En])
          localStorage.setItem("currantLang", LanguagesEnum[LanguagesEnum.Ar]);
        return JSON.parse(localStorage.getItem("currantLang") || '{}');
      }

      private useCurrent() {
        this.translate.use(this.getCurrentLang());
      }

  }