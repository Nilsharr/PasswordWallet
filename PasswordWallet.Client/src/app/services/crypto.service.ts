import { Injectable } from '@angular/core';
import { PasswordGeneratorOptions } from '../models/password-generator-options';

@Injectable({
  providedIn: 'root',
})
export class CryptoService {
  private readonly _crypto: Crypto = window.crypto;

  public randomPassword(options: PasswordGeneratorOptions): string {
    if (
      !options.length ||
      (!options.lowercase &&
        !options.uppercase &&
        !options.digits &&
        !options.special)
    ) {
      throw new Error('Cannot generate password with given requirements.');
    }
    const positions: string[] = [];
    let allCharSet = '';
    const lowercaseCharSet = 'abcdefghijklmnopqrstuvwxyz';
    const uppercaseCharSet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    const digitsCharSet = '0123456789';
    const specialCharSet = '!@#$%^&*-_()[]{}';

    if (options.lowercase) {
      allCharSet += lowercaseCharSet;
      positions.push('l');
    }
    if (options.uppercase) {
      allCharSet += uppercaseCharSet;
      positions.push('u');
    }
    if (options.digits) {
      allCharSet += digitsCharSet;
      positions.push('d');
    }
    if (options.special) {
      allCharSet += specialCharSet;
      positions.push('s');
    }
    while (positions.length < options.length!) {
      positions.push('a');
    }

    this.shuffleArray(positions);

    let password = '';
    for (let i = 0; i < positions.length; i++) {
      let positionChars: string = '';
      switch (positions[i]) {
        case 'l':
          positionChars = lowercaseCharSet;
          break;
        case 'u':
          positionChars = uppercaseCharSet;
          break;
        case 'd':
          positionChars = digitsCharSet;
          break;
        case 's':
          positionChars = specialCharSet;
          break;
        case 'a':
          positionChars = allCharSet;
          break;
        default:
          break;
      }

      const randomCharIndex = this.randomNumber(0, positionChars.length - 1);
      password += positionChars.charAt(randomCharIndex);
    }
    return password;
  }

  public randomNumber(min: number, max: number): number {
    let rval = 0;
    const range = max - min + 1;
    const bitsNeeded = Math.ceil(Math.log2(range));
    if (bitsNeeded > 53) {
      throw new Error('Cannot generate numbers larger than 53 bits.');
    }

    const bytesNeeded = Math.ceil(bitsNeeded / 8);
    const mask = Math.pow(2, bitsNeeded) - 1;
    const byteArray = new Uint8Array(this.randomBytes(bytesNeeded));

    let p = (bytesNeeded - 1) * 8;
    for (let i = 0; i < bytesNeeded; i++) {
      rval += byteArray[i] * Math.pow(2, p);
      p -= 8;
    }

    // Use & to apply the mask and reduce the number of recursive lookups
    rval = rval & mask;

    if (rval >= range) {
      // Integer out of acceptable range
      return this.randomNumber(min, max);
    }

    // Return an integer that falls within the range
    return min + rval;
  }

  public shuffleArray(array: string[]): void {
    for (let i = array.length - 1; i > 0; i--) {
      const j = this.randomNumber(0, i);
      [array[i], array[j]] = [array[j], array[i]];
    }
  }

  private randomBytes(length: number): Uint8Array {
    const arr = new Uint8Array(length);
    return this._crypto.getRandomValues(arr);
  }
}
