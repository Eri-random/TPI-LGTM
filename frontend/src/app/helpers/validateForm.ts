import { FormArray, FormControl, FormGroup } from "@angular/forms";

export default class ValidateForm {
    static validateAllFormFileds(formgroup: FormGroup | FormArray): void {
      Object.keys(formgroup.controls).forEach(field => {
        const control = formgroup.get(field);
        if (control instanceof FormControl) {
          control.markAsDirty({ onlySelf: true });
          control.updateValueAndValidity({ onlySelf: true });
        } else if (control instanceof FormGroup || control instanceof FormArray) {
          this.validateAllFormFileds(control);
        }
      });
    }
}